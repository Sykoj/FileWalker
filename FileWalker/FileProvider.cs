using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FileWalker {
    class FileProvider : IEnumerable<FileInfo>, IEnumerator<FileInfo> {

        IEnumerator FilesInDirectory { get; set; }
        Queue<DirectoryInfo> Directories { get; set; }

        public FileProvider(string directory) {

            if (!Directory.Exists(directory)) {
                throw new ArgumentException();
            }

            Directories = new Queue<DirectoryInfo>();

            var searchRoot = new DirectoryInfo(directory);
            FilesInDirectory = searchRoot.GetFiles().GetEnumerator();

            foreach (var subdirectory in searchRoot.GetDirectories()) {
                Directories.Enqueue(subdirectory);
            }
        }


        object IEnumerator.Current => _current;

        private FileInfo _current = null;
        FileInfo IEnumerator<FileInfo>.Current => _current;

        public void Dispose() {
            throw new NotImplementedException();
        }

        public IEnumerator<FileInfo> GetEnumerator() {
            return this;
        }

        public bool MoveNext() {

            if (FilesInDirectory.MoveNext()) {

                _current = (FileInfo) FilesInDirectory.Current;
                return true;
            }
            else {

                if (Directories.Count == 0) return false;
                DirectoryInfo newDirectory = null;

                try
                {

                    newDirectory = Directories.Dequeue();

                    foreach (var directory in newDirectory.GetDirectories())
                    {
                        Directories.Enqueue(directory);
                    }

                    FilesInDirectory = newDirectory.GetFiles().GetEnumerator();
                }
                catch (Exception) {
                }

                return MoveNext();
            }
        }

        public void Reset() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}