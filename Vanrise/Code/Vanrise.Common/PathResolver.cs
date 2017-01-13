using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public interface IResolver
    {
        IList<IPathElementFactory> PathElementFactories { get; set; }

        IList<IPathElement> CreatePath(string path);
        object Resolve(object target, IList<IPathElement> pathElements);
        object Resolve(object target, string path);
    }
    public class Resolver : IResolver
    {
        private IList<IPathElementFactory> pathElementFactories;
        /// <summary>
        /// contains the path element factories used to resolve given paths
        /// more specific factories must be before more generic ones, because the first applicable one is taken
        /// </summary>
        public IList<IPathElementFactory> PathElementFactories
        {
            get { return pathElementFactories; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("The PathElementFactories must not be null");
                pathElementFactories = value;
            }
        }

        public Resolver()
        {
            PathElementFactories = new List<IPathElementFactory>();
            PathElementFactories.Add(new PropertyFactory());
            PathElementFactories.Add(new EnumerableAccessFactory());
            PathElementFactories.Add(new DictionaryAccessFactory());
            PathElementFactories.Add(new SelectionFactory());
        }

        public IList<IPathElement> CreatePath(string path)
        {
            var pathElements = new List<IPathElement>();
            var tempPath = path;
            while (tempPath.Length > 0)
            {
                var pathElement = createPathElement(tempPath, out tempPath);
                pathElements.Add(pathElement);
                //remove the dots chaining properties 
                //no PathElement could do this reliably
                //the only appropriate one would be Property, but there doesn't have to be a dot at the beginning (if it is the first PathElement, e.g. "Property1.Property2")
                //and I don't want that knowledge in PropertyFactory
                if (tempPath.StartsWith("."))
                    tempPath = tempPath.Remove(0, 1);
            }
            return pathElements;
        }

        public object Resolve(object target, string path)
        {
            var pathElements = CreatePath(path);
            return Resolve(target, pathElements);
        }

        public object Resolve(object target, IList<IPathElement> pathElements)
        {
            var tempResult = target;
            foreach (var pathElement in pathElements)
            {
                if (tempResult is Selection)
                    tempResult = pathElement.Apply((Selection)tempResult);
                else
                    tempResult = pathElement.Apply(tempResult);
            }

            var result = tempResult;
            if (result is Selection)
                return ((Selection)result).AsEnumerable();
            else
                return result;
        }

        private IPathElement createPathElement(string path, out string newPath)
        {
            //get the first applicable path element type
            var pathElementFactory = PathElementFactories.Where(f => f.IsApplicable(path)).FirstOrDefault();

            if (pathElementFactory == null)
                throw new InvalidOperationException("There is no applicable path element factory for {path}");

            IPathElement result = pathElementFactory.Create(path, out newPath);
            return result;
        }
    }
    public class Selection : IEnumerable, IEnumerable<object>
    {
        public IReadOnlyCollection<object> Entries { get; set; }

        public Selection(IEnumerable entries)
        {
            var list = new List<object>();
            foreach (var entry in entries)
                list.Add(entry);
            Entries = list;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SelectionEnumerator(this);
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return new SelectionEnumerator(this);
        }
    }
    public class SelectionEnumerator : IEnumerator, IEnumerator<object>
    {
        private Selection selection;
        private int currentIndex;

        public SelectionEnumerator(Selection selection)
        {
            this.selection = selection;
            currentIndex = -1;
        }

        public object Current
        {
            get
            {
                lock (selection)
                {
                    if (currentIndex == -1) return null;
                    if (currentIndex >= selection.Entries.Count)
                        throw new InvalidOperationException("The enumerator is past the last element. Call Reset to start again from the first one");
                    return selection.Entries.ElementAt(currentIndex);
                }
            }
        }

        public bool MoveNext()
        {
            currentIndex += 1;
            return currentIndex < selection.Entries.Count;
        }

        public void Reset()
        {
            currentIndex = -1;
        }

        public void Dispose()
        {
        }
    }
    public class DictionaryAccess : PathElementBase
    {
        private string key;

        public DictionaryAccess(string key)
        {
            this.key = key;
        }

        public override object Apply(object target)
        {
            var dictionary = target as IDictionary;
            foreach (DictionaryEntry de in dictionary)
            {
                if (de.Key.ToString() == key)
                    return de.Value;
            }

            //if no value is returned by now, it means that the index is too high
            throw new ArgumentException("The key {key} does not exist.");
        }
    }
    public class DictionaryAccessFactory : IPathElementFactory
    {
        public IPathElement Create(string path, out string newPath)
        {
            var matches = Regex.Matches(path, @"^\[(\w+)\]");
            Match match = matches[0];
            //0 is the whole match
            string key = match.Groups[1].Value; //the regex guarantees that the second group is an integer, so no further check is needed
            newPath = path.Remove(0, match.Value.Length);
            return new DictionaryAccess(key);
        }

        public bool IsApplicable(string path)
        {
            return Regex.IsMatch(path, @"^\[\w+\]");
        }
    }
    public class EnumerableAccess : PathElementBase
    {
        private int index;

        public EnumerableAccess(int index)
        {
            this.index = index;
        }

        public override object Apply(object target)
        {
            //index lower than 0 doesn't have to be checked, because the IsApplicable check doesn't apply to negative values

            var enumerable = target as IEnumerable;

            var i = 0;
            foreach (var value in enumerable)
            {
                if (i == index)
                    return value;
                i++;
            }

            //if no value is returned by now, it means that the index is too high
            throw new IndexOutOfRangeException("The index {index} is too high. Maximum index is {i - 1}.");
        }
    }
    public class EnumerableAccessFactory : IPathElementFactory
    {
        public IPathElement Create(string path, out string newPath)
        {
            var matches = Regex.Matches(path, @"^\[(\d+)\]");
            Match match = matches[0];
            //0 is the whole match
            int index = int.Parse(match.Groups[1].Value); //the regex guarantees that the second group is an integer, so no further check is needed
            newPath = path.Remove(0, match.Value.Length);
            return new EnumerableAccess(index);
        }

        public bool IsApplicable(string path)
        {
            return Regex.IsMatch(path, @"^\[\d+\]");
        }
    }
    public interface IPathElement
    {
        object Apply(object target);
        IEnumerable Apply(Selection target);
    }
    public interface IPathElementFactory
    {
        /// <summary>
        /// checks if the factory can create a path element from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsApplicable(string path);

        /// <summary>
        /// creates a path element from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newPath">outputs the path removed by the bit that was used to create the path element</param>
        /// <returns></returns>
        IPathElement Create(string path, out string newPath);
    }
    public abstract class PathElementBase : IPathElement
    {
        public IEnumerable Apply(Selection target)
        {
            var results = new List<object>();
            foreach (var entriy in target.Entries)
            {
                results.Add(Apply(entriy));
            }
            var result = new Selection(results);
            return result;
        }

        public abstract object Apply(object target);
    }
    public class Property : PathElementBase
    {
        private string property;

        public Property(string property)
        {
            this.property = property;
        }

        public override object Apply(object target)
        {
            PropertyInfo p = target.GetType().GetTypeInfo().GetDeclaredProperty(property);
            if (p == null)
                throw new ArgumentException("The property {property} could not be found.");

            var result = p.GetValue(target);
            return result;
        }
    }
    public class PropertyFactory : IPathElementFactory
    {
        public IPathElement Create(string path, out string newPath)
        {
            string property = Regex.Matches(path, @"^\w+")[0].Value;
            newPath = path.Remove(0, property.Length);
            return new Property(property);
        }

        public bool IsApplicable(string path)
        {
            return Regex.IsMatch(path, @"^\w+");
        }
    }
    public class SelectionAccess : IPathElement
    {
        public SelectionAccess()
        {
        }

        public object Apply(object target)
        {
            var enumerable = target as IEnumerable;
            var result = new Selection(enumerable);
            return result;
        }

        public IEnumerable Apply(Selection target)
        {
            var results = new List<object>();
            foreach (var entry in target.Entries)
            {
                var enumerable = entry as IEnumerable;
                if (enumerable == null)
                    results.Add(entry);
                else
                {
                    foreach (var element in enumerable)
                        results.Add(element);
                }
            }
            var result = new Selection(results);
            return result;
        }
    }
    public class SelectionFactory : IPathElementFactory
    {
        private const string selectionIndicator = "[]";
        public IPathElement Create(string path, out string newPath)
        {
            newPath = path.Remove(0, selectionIndicator.Length);
            return new SelectionAccess();
        }

        public bool IsApplicable(string path)
        {
            return path.StartsWith(selectionIndicator);
        }
    }
}
