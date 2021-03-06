﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Learn.Model
{
    /// <summary>
    /// Tanfolyamok tároló osztálya
    /// </summary>
    public class CourseRepository:Notifier
    {
        private readonly ObservableCollection<Course> _courseList;
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CourseRepository(string directory)
        {
            Directory = directory;
            _courseList = new ObservableCollection<Course>();
        }
        /// <summary>
        /// Tanfolyam fájlok könyvtára
        /// </summary>
        public string Directory
        {
            get;
            set;
        }
        /// <summary>
        /// Tanfolyamok listája
        /// </summary>
        public ObservableCollection<Course> CourseList
        {
            get { return _courseList; }
        }
        /// <summary>
        /// Indexelővel is el lehet érni a tanfolyamokat
        /// </summary>
        /// <param name="idx">index</param>
        /// <returns></returns>
        public Course this[int idx]
        {
            get
            {
                if (idx >= 0 && idx < _courseList.Count)
                    return _courseList[idx];
                else
                    return null;
            }
            set
            {
                if (idx >= 0 && idx < _courseList.Count)
                {
                    _courseList[idx] = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Paraméterben kapott tanfolyam mentése
        /// </summary>
        /// <param name="course">Tanfolyam</param>
        private void Save(Course course)
        {
            if (string.IsNullOrEmpty(course.FileName))
                throw new Exception("Nincs megadva fájlnév");
            if (string.IsNullOrEmpty(Directory))
                throw new Exception("Nincs megadva a könyvtár");
            using (StreamWriter writer = new StreamWriter(Path.Combine(Directory,course.FileName)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Course));
                serializer.Serialize(writer, course);
            }
        }
        /// <summary>
        /// Elmenti az összes tanfolyamot
        /// </summary>
        public void SaveAll()
        {
            int i = 0;
            foreach (var course in CourseList)
            {
                if (string.IsNullOrEmpty(course.FileName))
                {
                    do
                    {
                        course.FileName = Path.Combine(Directory, string.Format("course{0}.xml", i++));
                    } while (File.Exists(course.FileName));
                }
                Save(course);
            }
        }
        /// <summary>
        /// Paraméterben kapott fájlnevű lecke betöltése
        /// </summary>
        /// <param name="FileName">Fájlnév</param>
        /// <returns></returns>
        private void Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("Nincs megadva fájlnév");
            if (string.IsNullOrEmpty(Directory))
                throw new Exception("Nincs megadva a könyvtár");
            using (StreamReader reader = new StreamReader(Path.Combine(Directory,fileName)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Course));
                Course course = (Course)serializer.Deserialize(reader);
                course.FileName = fileName;
                _courseList.Add(course);
                NotifyPropertyChanged();
            }
        }

        public void LoadAll()
        {
            foreach (var file in System.IO.Directory.GetFiles(Directory,"course*.xml"))
            {
                Load(file);
                NotifyPropertyChanged();
            }
        }

        public void Reload(Course course)
        {
            Load(course.FileName);
        }
    }
}
