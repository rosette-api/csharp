using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rosette_api
{
    /// <summary>
    /// A class to make Objects optional
    /// </summary>
    /// <typeparam name="T">The Type of the object</typeparam>
    public class Optional<T>
    {
        /// <summary>
        /// Does this object have a value?
        /// </summary>
        public bool HasValue { get; private set; }
        private T value;
        /// <summary>
        /// Gets the object's value if one exists
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws an InvalidOperationException if the value does not exist</exception>
        public T Value
        {
            get
            {
                if (HasValue)
                    return value;
                else
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Creates an Optional Object with a value
        /// </summary>
        /// <param name="value">The value of the object</param>
        public Optional(T value)
        {
            this.HasValue = true;
            this.value = value;
        }

        /// <summary>
        /// Gets the value of the optional object
        /// </summary>
        /// <param name="optional">The optional Object</param>
        /// <returns>The value of the optional Object</returns>
        public static explicit operator T(Optional<T> optional)
        {
            return optional.Value;
        }

        /// <summary>
        /// Gets the optional version of an object
        /// </summary>
        /// <param name="value">An object</param>
        /// <returns>The Optional version of the given value</returns>
        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }

        /// <summary>
        /// Is this Optional Object equal to the given Object?
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns>True if they are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Optional<T>) 
            {
                return this.Equals((Optional<T>)obj);
            }
            else if (HasValue)
            {
                return this.Value.Equals(obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is this Optional Object equal to the other Optional Object?
        /// </summary>
        /// <param name="other">The other Optional Object</param>
        /// <returns>True if they are equal</returns>
        public bool Equals(Optional<T> other)
        {
            if (HasValue && other.HasValue)
            {
                return object.Equals(value, other.value);
            }
            else
            {
                return HasValue == other.HasValue;
            }
        }

        /// <summary>
        /// Gets the HashCode of this Optional Object.
        /// </summary>
        /// <returns>The hashcode of the value of this Optional Object or 0 if it doesn't have a value</returns>
        public override int GetHashCode()
        {
            if (this.HasValue)
            {
                return this.value.GetHashCode();
            }
            else
            {
                return 0;
            }
        }
    }
}
