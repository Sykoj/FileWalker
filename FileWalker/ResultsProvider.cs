using System;
using System.Collections.Immutable;

namespace FileWalker {
    public class ResultsProvider : ViewUpdateableObject {
        
        private TimeSpan _currentTime = TimeSpan.Zero;
        private DateTime _startTime;

        private string _searchState = "waiting to start";
        private string _searchedWord;
        private double _processedMegabytes;
        private int _visitedFiles;
        private int _matchingFiles;
        private ImmutableSortedSet<string> _matchingFilenames;
        private int _errorNonAccessedFiles;

        public object CurrentTime {
 
            get { return Convert.ToString(_currentTime); }
            set {
                _currentTime = (DateTime) value - _startTime;
                Notify();
            }
        }

        public string SearchState {
            get { return _searchState; }
            set { _searchState = value; Notify(); }
        }

        public string SearchedWord {
            get { return _searchedWord; }
            set { _searchedWord = value; Notify(); }
        }
        
        public double ProcessedMegabytes {
            get { return _processedMegabytes; }
            set { _processedMegabytes = value; Notify(); }
        }

        public int VisitedFiles {
            get { return _visitedFiles; }
            set { _visitedFiles = value; Notify(); }
        }

        public int MatchingFiles {
            get { return _matchingFiles; }
            set { _matchingFiles = value; Notify(); }
        }

        public ImmutableSortedSet<string> MatchingFilenames {
            get { return _matchingFilenames; }
            set { _matchingFilenames = value; Notify(); }
        }

        public int ErrorNonAccessedFiles {
            get { return _errorNonAccessedFiles; }
            set { _errorNonAccessedFiles = value; Notify(); }
        }
        
        public void StartTimer() {
            _startTime = DateTime.Now;
        }
    }
}