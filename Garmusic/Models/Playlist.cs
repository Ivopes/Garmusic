using System;
using System.Collections;
using System.Collections.Generic;

namespace API_BAK.Models
{
    public class Playlist : IList<string>
    {
        private List<string> songs = new List<string>();
        public string this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => songs.Count;

        public bool IsReadOnly => false;

        public void Add(string item)
        {
            songs.Add(item);
        }
        public void Clear()
        {
            songs.Clear();
        }
        public bool Contains(string item)
        {
            return songs.Contains(item);
        }
        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<string> GetEnumerator()
        {
            foreach (var s in songs)
            {
                yield return s;
            }
        }
        public int IndexOf(string item)
        {
            throw new NotImplementedException();
        }
        public void Insert(int index, string item)
        {
            throw new NotImplementedException();
        }
        public bool Remove(string item)
        {
            throw new NotImplementedException();
        }
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
