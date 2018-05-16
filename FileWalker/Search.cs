using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Reflection;

namespace FileWalker {
    public class Query : ViewUpdateableObject {
        
        private IDispatcher Dispatcher { get; set; }
        private IDispatcherTimer DispatcherTimer { get; }
        
        public FileProcessor FileProcessor { get; set; }
        private Queue<FileInfo> Queue { get; }
        private IEnumerator<FileInfo> Files { get; }
        private int QueueSize { get; }

        public Query(string searchedWord, string directory, int queueSize, IDispatcher dispatcher, IDispatcherTimer dispatcherTimer) {
 
            Dispatcher = dispatcher;
            DispatcherTimer = dispatcherTimer;
            DispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            
            FileProcessor = new FileProcessor(searchedWord, Dispatcher);
            
            DispatcherTimer.Handler = (sender, args) => {
                FileProcessor.Results.CurrentTime = DateTime.Now;
            };
            
            Files = new FileProvider(directory).GetEnumerator();
            QueueSize = queueSize;
            Queue = new Queue<FileInfo>();
        }

        public void Process(int searcherCount) {
            
            Dispatcher.InvokeAsync(() => {

                FileProcessor.Results.SearchState = "searching";
                FileProcessor.Results.StartTimer();
                DispatcherTimer.Start();
            });
            
            var producerThread = new Thread(() => {

                while (true) {

                    var file = Files.MoveNext() ? Files.Current : null;

                    lock (Queue) {

                        while (Queue.Count > QueueSize) Monitor.Wait(Queue);

                        Queue.Enqueue(file);

                        if (file == null) {

                            Monitor.PulseAll(Queue);
                            break;
                        }
                        else {
                            Monitor.Pulse(Queue);
                        }
                    }
                }
            });

            ThreadStart searcher = () => {

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

                    // out of lock
                    FileProcessor.Process(item);
                }
            };
            
            Thread[] threads = new Thread[searcherCount];

            for (var i = 0; i < threads.Length; ++i) {
                threads[i] = new Thread(searcher);
                threads[i].Start();
            }

            producerThread.Start();

            producerThread.Join();
            
            for (var i = 0; i < threads.Length; ++i) {
                threads[i].Join();
            }

            
            Dispatcher.InvokeAsync(() =>
            {
                FileProcessor.Results.SearchState = "finished";
                DispatcherTimer.Stop();
            });
        }
    }
}