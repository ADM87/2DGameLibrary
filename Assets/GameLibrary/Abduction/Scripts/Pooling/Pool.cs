using System.Collections.Generic;

namespace Abduction.Pooling
{
    public class Pool<TItem>
    {
        public delegate TItem PoolAllocator();
        public delegate void PoolDeallocator(TItem item);

        private Queue<TItem> free;

        private PoolAllocator allocator;
        private PoolDeallocator deallocator;

        public Pool(PoolAllocator alloc, PoolDeallocator dealloc)
        {
            free = new Queue<TItem>();

            allocator = alloc;
            deallocator = dealloc;
        }

        public TItem Take()
        {
            if (free.Count > 0)
                return free.Dequeue();

            return allocator();
        }

        public void Put(TItem item)
        {
            free.Enqueue(item);
        }

        public void Empty()
        {
            while (free.Count > 0)
                deallocator(free.Dequeue());
        }
    }
}
