namespace SerialService.Infrastructure.Core
{
    using System.Collections.Generic;
    using System.Collections;
    using DAL.Entities;
    using System;

    public class EntityList<T> : IList<T> where T : IBaseEntity
    {
        /// <summary>
        /// Инициализирует новый пустой объект в памяти.
        /// </summary>
        public EntityList()
        {
            this.items = EntityList<T>.emptyArray;
        }

        /// <summary>
        /// Инициализирует новый объект в памяти и копирует элементы из collection.
        /// </summary>
        /// <param name="collection">Коллекция для копирования элементов.</param>
        public EntityList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            ICollection col = collection as ICollection;
            if(col != null)
            {
                int count = col.Count;

                if (count == 0)
                    this.items = EntityList<T>.emptyArray;
                else
                {
                    this.items = new T[count];
                    col.CopyTo(this.items, 0);
                    this.Count = count;
                }
            }
            else
            {
                this.Count = 0;
                this.items = EntityList<T>.emptyArray;

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while(en.MoveNext())
                    {
                        this.Add(en.Current);
                    }
                }
            }
        }
        
        /// <summary>
        /// Добавить новый объект к коллекции.
        /// </summary>
        /// <param name="item">Новый объект.</param>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException();

            if (this.Count == this.items.Length)
                EnsureCapacity(this.Count + 1);

            this.items[this.Count++] = item;
        }

        /// <summary>
        /// Удаляет все объекты из коллекции.
        /// </summary>
        public void Clear()
        {
            if(this.Count > 0)
            {
                Array.Clear(this.items, 0, this.Count);
                this.Count = 0;
            }
        }

        /// <summary>
        /// Проверяет наличие объекта по условию, заданному в методе IBaseEntity.Alike()
        /// </summary>
        /// <param name="item">Элемент для проверки.</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            if ((Object)item == null)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if ((Object)this.items[i] == null)
                        return true;
                }

                return false;
            }
            else
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if(this.items[i].Alike(item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Копирует все элементы в массив array.
        /// </summary>
        /// <param name="array">Массив, куда копировать элементы.</param>
        /// <param name="arrayIndex">Индекс, начиная с которого вставлять в массив элементы.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this.items, 0, array, arrayIndex, this.Count);
        }

        /// <summary>
        /// Ищет указанный элемент в коллекция и возвращает его индекс.
        /// </summary>
        /// <param name="item">Элемент для поиска.</param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(this.items, item, 0, this.Count);
        }

        /// <summary>
        /// Вставить в коллекцию элемент с указанным индексом.
        /// </summary>
        /// <param name="index">Индекс для вставки.</param>
        /// <param name="item">Элемент.</param>
        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)this.Count)
                throw new ArgumentOutOfRangeException();

            if (this.Count == this.items.Length)
                EnsureCapacity(this.Count + 1);

            if (index < this.Count)
                Array.Copy(this.items, index, this.items, index + 1, this.Count - index);

            this.items[index] = item;
            this.Count++;
        }

        /// <summary>
        /// Удаляет указанный элемент из коллекции.
        /// </summary>
        /// <param name="item">Элемент.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            int index = this.IndexOf(item);

            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Удаляет элемент, располагающийся по указанному индексу.
        /// </summary>
        /// <param name="index">Индекс для удаления.</param>
        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)this.Count)
                throw new ArgumentOutOfRangeException();

            this.Count--;
            if (index < this.Count)
                Array.Copy(this.items, index + 1, this.items, index, this.Count - index);

            this.items[this.Count] = default(T);
        }

        /// <summary>
        /// Возвращает енумератор.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
                yield return this.items[i];
               
        }

        /// <summary>
        /// Возвращает енумератор.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Выделить память под котейнер.
        /// </summary>
        /// <param name="min"></param>
        private void EnsureCapacity(int min)
        {
            if (this.items.Length < min)
            {
                int newCapacity = this.items.Length == 0 ? EntityList<T>.defaultCapacity : this.items.Length * 2;
                //if ((uint)newCapacity > Array.MaxArrayLength) newCapacity = Array.MaxArrayLength;
                if (newCapacity < min) newCapacity = min;
                this.Capacity = newCapacity;
            }
        }

        /// <summary>
        /// Возвращает элемент по указанному индексу.
        /// </summary>
        /// <param name="index">Индекс.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)this.Count)
                    throw new ArgumentOutOfRangeException();

                return this.items[index];
            }
            set
            {
                if ((uint)index >= (uint)this.Count)
                    throw new ArgumentOutOfRangeException();

                this.items[index] = value;
            }
        }

        private const int defaultCapacity = 4; // изначальная вместимость контейнера.
        private T[] items; // коллекция для хранения объектов.
        static readonly T[] emptyArray = new T[0]; // заготовка пустой коллекции.

        /// <summary>
        /// Возвращает или задает количество элементов контейнера.
        /// </summary>
        public int Count
        {
            get;
            private set;
        }
        /// <summary>
        /// Контейнер только для чтения? Всегда возвращает false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Возвращает или задает вместимость контейнера.
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.items.Length;
            }
            set
            {
                if (value < this.Count)
                    throw new ArgumentOutOfRangeException();

                if (value != this.items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (this.Count > 0)
                            Array.Copy(this.items, 0, newItems, 0, this.Count);

                        this.items = newItems;
                    }
                    else
                    {
                        this.items = EntityList<T>.emptyArray;
                    }
                }
            }
        }
    }
}