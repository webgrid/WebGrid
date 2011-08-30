/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    //Creating a Set Object
    /// <summary>
    /// Creating a set object
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    [Serializable]
    public class Set<T> : IEnumerable<T>
    {
        #region Fields

        private readonly bool dictionaryLookup;

        /// <summary>
        /// We use dictionary for lookup for simple classes like 'int','string','float'++
        /// </summary>
        private readonly Dictionary<int, T> internalDic = new Dictionary<int, T>();
        private readonly List<T> internalSet = new List<T>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Standard Set class where we are lookup against a generic list.
        /// </summary>
        public Set()
        {
        }

        /// <summary>
        /// Standard Set class where we are lookup against a generic list if Hash
        /// </summary>
        /// <param name="useHashLookup">if </param>
        public Set(bool useHashLookup)
        {
            dictionaryLookup = useHashLookup;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Returns number of elements in the set object.
        /// </summary>
        public int Count
        {
            get { return (internalSet.Count); }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Returns type
        /// </summary>
        /// <param name="index">index of the object</param>
        /// <returns>The object</returns>
        public T this[int index]
        {
            get
            {
                return (internalSet[index]);
            }
            set
            {
                if (internalSet.Contains(value))
                {
                    throw (new ArgumentException(
                           "Duplicate object cannot be added to this set.", "index"));
                }
                internalSet[index] = value;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Returns a set intersection
        /// </summary>
        /// <param name="lhs">left set </param>
        /// <param name="rhs">right set </param>
        /// <returns>Intersection of left and right set</returns>
        public static Set<T> operator &(Set<T> lhs, Set<T> rhs)
        {
            return (lhs.IntersectionOf(rhs));
        }

        /// <summary>
        /// Returns a difference set.
        /// </summary>
        /// <param name="lhs">left set</param>
        /// <param name="rhs">right set</param>
        /// <returns>Difference of left and right set </returns>
        public static Set<T> operator ^(Set<T> lhs, Set<T> rhs)
        {
            return (lhs.DifferenceOf(rhs));
        }

        /// <summary>
        /// Returns an union of the Set object.
        /// </summary>
        /// <param name="lhs">left set</param>
        /// <param name="rhs">right set</param>
        /// <returns>Union of left and right set</returns>
        public static Set<T> operator |(Set<T> lhs, Set<T> rhs)
        {
            return (lhs.UnionOf(rhs));
        }

        /// <summary>
        /// Adds an object at the end of the collection.
        /// </summary>
        /// <param name="obj">the object</param>
        public void Add(T obj)
        {
            if (internalSet.Contains(obj))
            {
                throw (new ArgumentException(
                       "Duplicate object cannot be added to this set.", "obj"));
            }
            internalDic.Add(obj.GetHashCode(), obj);
            internalSet.Add(obj);
        }

        /// <summary>
        /// Removes all elements from the Set collection
        /// </summary>
        public void Clear()
        {
            internalSet.Clear();
            internalDic.Clear();
        }

        /// <summary>
        /// Indicates whether an object is in the collection.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(T obj)
        {
            return dictionaryLookup ? internalDic.ContainsKey(obj.GetHashCode()) : (internalSet.Contains(obj));
        }

        /// <summary>
        /// Returns a difference set.
        /// </summary>
        /// <param name="set">The set object</param>
        /// <returns>Difference of left and right set </returns>
        public Set<T> DifferenceOf(Set<T> set)
        {
            Set<T> differenceSet = new Set<T>();

            // mergeSet XOR sourceSet

            for (int index = 0; index < set.Count; index++)
            {
                if (!Contains(set.internalSet[index]))
                {
                    differenceSet.Add(set.internalSet[index]);
                }
            }
            for (int index = 0; index < Count; index++)
            {
                if (!set.Contains(internalSet[index]))
                {
                    differenceSet.Add(internalSet[index]);
                }
            }
            return (differenceSet);
        }

        /// <summary>
        /// Returns a string represetation of the set.
        /// </summary>
        /// <returns>a string represetation</returns>
        public string DisplaySet()
        {
            if (Count == 0)
            {
                return ("{}");
            }
            StringBuilder displayStr = new StringBuilder("{ ");

            for (int index = 0; index < (Count - 1); index++)
            {
                displayStr.Append(internalSet[index]);
                displayStr.Append(", ");
            }
            displayStr.Append(internalSet[internalSet.Count - 1]);
            displayStr.Append(" }");

            return (displayStr.ToString());
        }

        /* /// <summary>
        /// Indicates if left and right set are the same
        /// </summary>
        /// <param name="lhs">left set </param>
        /// <param name="rhs">right set </param>
        /// <returns>true if equal else false</returns>
        public static bool operator == (Set<T> lhs, Set<T> rhs)
        {
            if (lhs == null)
                return false;
            return (lhs.Equals(rhs));
        }
        /// <summary>
        /// Indicates if left and right set are different
        /// </summary>
        /// <param name="lhs">left set </param>
        /// <param name="rhs">right set </param>
        /// <returns>true if false else true</returns>
        public static bool operator !=(Set<T> lhs, Set<T> rhs)
        {
            if (lhs == null)
                return false;
            return (!lhs.Equals(rhs));
        }*/
        /// <summary>
        /// Indicates if the objects has same items in their collections. (In unordered order)
        /// </summary>
        /// <param name="obj">Set object</param>
        /// <returns>true if same elements in the collections</returns>
        public override bool Equals(object obj)
        {
            bool isEquals = false;

            if (obj != null)
            {
                if (obj is Set<T>)
                {
                    if (Count == ((Set<T>) obj).Count && (IsSubsetOf((Set<T>) obj) &&
                                                          ((Set<T>) obj).IsSubsetOf(this)))
                    {
                        isEquals = true;
                    }
                }
            }
            return (isEquals);
        }

        /// <summary>
        /// Returns an enumerator of the set object.
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            for (int cntr = 0; cntr < internalSet.Count; cntr++)
            {
                yield return (internalSet[cntr]);
            }
        }

        /// <summary>
        /// Returns hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (internalSet.GetHashCode());
        }

        /// <summary>
        /// Returns an enumerator of a specific type of the set object.
        /// </summary>
        /// <returns>enumerator of a specific type</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int cntr = 0; cntr < internalSet.Count; cntr++)
            {
                yield return (internalSet[cntr]);
            }
        }

        /// <summary>
        /// Returns a set intersection
        /// </summary>
        /// <param name="set">The set object</param>
        /// <returns>Intersection of left and right set</returns>
        public Set<T> IntersectionOf(Set<T> set)
        {
            Set<T> intersectionSet = new Set<T>();
            Set<T> sourceSet;
            Set<T> mergeSet;

            if (set.Count > Count) // An optimization
            {
                sourceSet = set;
                mergeSet = this;
            }
            else
            {
                sourceSet = this;
                mergeSet = set;
            }

            // mergeSet AND sourceSet
            for (int index = 0; index < mergeSet.Count; index++)
            {
                if (sourceSet.Contains(mergeSet.internalSet[index]))
                {
                    intersectionSet.Add(mergeSet.internalSet[index]);
                }
            }

            return (intersectionSet);
        }

        /// <summary>
        /// Indicates if this object is a sub set (unordered).
        /// </summary>
        /// <param name="set">The set object</param>
        /// <returns>true if subset else false</returns>
        public bool IsSubsetOf(Set<T> set)
        {
            for (int index = 0; index < Count; index++)
                if (!set.Contains(internalSet[index]))
                    return false;
            return true;
        }

        /// <summary>
        /// Indicates if this object is a Supersetof (unordered).
        /// </summary>
        /// <param name="set">The set object</param>
        /// <returns>true if subset else false</returns>
        public bool IsSupersetOf(Set<T> set)
        {
            for (int index = 0; index < set.Count; index++)
            {
                if (!Contains(set.internalSet[index]))
                {
                    return (false);
                }
            }
            return (true);
        }

        /// <summary>
        /// Remove object from the collection.
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(T obj)
        {
            if (!internalSet.Contains(obj))
                throw (new ArgumentException("Object cannot be removed from this set because it does not exist in this set.", "obj"));
            internalSet.Remove(obj);
            internalDic.Remove(obj.GetHashCode());
        }

        /// <summary>
        /// Remove object at a specific index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            internalSet.RemoveAt(index);
        }

        /// <summary>
        /// Returns an union of the Set object.
        /// </summary>
        /// <param name="set">The set object</param>
        /// <returns>Union of left and right set</returns>
        public Set<T> UnionOf(Set<T> set)
        {
            Set<T> unionSet = new Set<T>();
            Set<T> sourceSet;
            Set<T> mergeSet;

            if (set.Count > Count) // An optimization
            {
                sourceSet = set;
                mergeSet = this;
            }
            else
            {
                sourceSet = this;
                mergeSet = set;
            }
            // Initialize unionSet with the entire SourceSet.
            for (int index = 0; index < sourceSet.Count; index++)
            {
                unionSet.Add(sourceSet.internalSet[index]);
            }

            // mergeSet OR sourceSet
            for (int index = 0; index < mergeSet.Count; index++)
            {
                if (!sourceSet.Contains(mergeSet.internalSet[index]))
                {
                    unionSet.Add(mergeSet.internalSet[index]);
                }
            }

            return (unionSet);
        }

        #endregion Methods
    }
}