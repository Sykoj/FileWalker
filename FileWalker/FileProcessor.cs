using System.IO;
using System.Text;
using System.Collections.Immutable;
using System;
using Cuni.NPrg038;

namespace FileWalker {
    public class FileProcessor {
        private IDispatcher Dispatcher {get;set;}
        public ResultsProvider Results { get; set; }

        private AhoCorasickSearch SearchAlgorithm { get; set; }

        public FileProcessor(string searchedWord, IDispatcher dispatcher) {

            Dispatcher = dispatcher;

            SearchAlgorithm = new AhoCorasickSearch();
            Results = new ResultsProvider() {

                SearchedWord = searchedWord,
                MatchingFilenames = ImmutableSortedSet<string>.Empty
            };

            SearchAlgorithm.AddPattern(Encoding.Unicode.GetBytes(searchedWord));
            SearchAlgorithm.AddPattern(Encoding.UTF8.GetBytes(searchedWord));
            SearchAlgorithm.AddPattern(Encoding.UTF32.GetBytes(searchedWord));
            SearchAlgorithm.Freeze();
        }

        public void Process(FileInfo info) {

            var isMatching = FileMatching(info);

            Dispatcher.InvokeAsync(() => {

                if (isMatching) {
                    Results.MatchingFiles++;
                    Results.MatchingFilenames = Results.MatchingFilenames.Add(info.Name);
                }
                
                Results.ProcessedMegabytes += (double) info.Length / (1024 * 1024); // to MiB
                Results.VisitedFiles++;
            });
        }

        private bool FileMatching(FileInfo info) {

            var search = SearchAlgorithm.InitialState;

            try {
                using (var fileStream = info.Open(FileMode.Open, FileAccess.Read)) {

                    int byteValue;
                    while ((byteValue = fileStream.ReadByte()) != -1) {
                        search = search.GetNextState((byte) byteValue);

                        if (search.HasMatchedPattern) return true;
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
