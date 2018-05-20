using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Text;
using Cuni.NPrg038;
using System.Collections.Immutable;

namespace FileWalker {
    public class Query : ViewUpdateableObject {
        
        private IDispatcher Dispatcher { get; set; }
        private IDispatcherTimer DispatcherTimer { get; }
        
        private Queue<FileInfo> Queue { get; }
        private IEnumerator<FileInfo> Files { get; }
        private bool AbortRequested { get; set; } = false;
        private int QueueSize { get; }
        private ResultsProvider Results { get; }

        private AhoCorasickSearch SearchAlgorithm { get; }
        
        public Query(string searchedWord, string directory, int queueSize, IDispatcher dispatcher, IDispatcherTimer dispatcherTimer) {
 
            Dispatcher = dispatcher;
            DispatcherTimer = dispatcherTimer;
            DispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            
            DispatcherTimer.Handler = (sender, args) => {
                Results.CurrentTime = DateTime.Now;
            };
            
            Files = new FileProvider(directory).GetEnumerator();
            QueueSize = queueSize;
            Queue = new Queue<FileInfo>();
            
            SearchAlgorithm = new AhoCorasickSearch();
            SearchAlgorithm.AddPattern(Encoding.Unicode.GetBytes(searchedWord));
            SearchAlgorithm.AddPattern(Encoding.UTF8.GetBytes(searchedWord));
            SearchAlgorithm.AddPattern(Encoding.UTF32.GetBytes(searchedWord));
            SearchAlgorithm.Freeze();
            
            Results = new ResultsProvider() {

                SearchedWord = searchedWord,
                MatchingFilenames = ImmutableSortedSet<string>.Empty
            };
            
        }

        public void Process(int searcherCount) {
            
            Dispatcher.InvokeAsync(() => {

                Results.SearchState = "searching";
                Results.StartTimer();
                DispatcherTimer.Start();
            });
            
            Thread[] threads = new Thread[searcherCount];
            var producerThread = GetProducerThread();
            
            for (var i = 0; i < threads.Length; ++i) {
                threads[i] = new Thread(GetSearcher());
                threads[i].Start();
            }
            
            producerThread.Start();
         
            producerThread.Join();
            for (var i = 0; i < threads.Length; ++i) {
                threads[i].Join();
            }
            
            Dispatcher.InvokeAsync(() =>
            {
                Results.SearchState = "finished";
                DispatcherTimer.Stop();
            });
        }

        public void AbortSearch() {
            
            AbortRequested = true;
        }

        private Thread GetProducerThread() {
            
            return new Thread(() => {

                while (true) {

                    var file = Files.MoveNext() ? Files.Current : null;

                    lock (Queue) {

                        while (Queue.Count > QueueSize) {
                            Monitor.Wait(Queue);
                        }

                        if (file == null || AbortRequested) {

                            Queue.Enqueue(null);
                            Monitor.PulseAll(Queue);
                            break;
                        }
                        else {
                            Queue.Enqueue(file);
                            Monitor.Pulse(Queue);
                        }
                    }

                }
            });
        }
        
        private ThreadStart GetSearcher() {

            return () => {

                while (true) {

                    FileInfo item;

                    lock (Queue) {

                        while (Queue.Count == 0) {
                            Monitor.Wait(Queue);
                        }
                        
                        item = Queue.Peek();
                        if (item == null) {
                            break;
                        }

                        Queue.Dequeue();
                        Monitor.Pulse(Queue);
                    }

                    var isMatching = FileMatching(item);

                    if (AbortRequested) continue;
                    
                    Dispatcher.InvokeAsync(() => {

                        if (isMatching) {
                            Results.MatchingFiles++;
                            Results.MatchingFilenames = Results.MatchingFilenames.Add(item.Name);
                        }
                
                        Results.ProcessedMegabytes += (double) item.Length / (1024 * 1024); // to MiB
                        Results.VisitedFiles++;
                    });                    
                }
            };
        }
        
        private bool FileMatching(FileInfo info) {

            var search = SearchAlgorithm.InitialState;

            try {
                using (var fileStream = info.Open(FileMode.Open, FileAccess.Read)) {

                    int byteValue;
                    while ((byteValue = fileStream.ReadByte()) != -1) {
                        search = search.GetNextState((byte) byteValue);

                        if (search.HasMatchedPattern) return true;
                        if (AbortRequested) return false;
                    }

                }
            }
            catch (Exception) {
   
                Dispatcher.InvokeAsync(() => {
                    Results.ErrorNonAccessedFiles++;
                });
            }

            return false;
        }        
    }
}