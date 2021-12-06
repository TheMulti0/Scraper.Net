namespace PostsListener.Client.Tests
{
    internal class NewPostCounter
    {
        private int _count = 0;

        public int Get()
        {
            lock (this)
            {
                return _count;
            }
        }

        public void Increment()
        {
            lock (this)
            {
                _count++;
            }
        }
    }
}