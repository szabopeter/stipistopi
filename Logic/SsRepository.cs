using System.Collections.Generic;
using System.Linq;

namespace logic
{
    public interface ISsRepository
    {
        void Add(SsResource ssResource);
        List<SsResource> GetAll();
    }

    public class InMemorySsRepository : ISsRepository
    {
        private readonly List<SsResource> _resources = new List<SsResource>();

        public void Add(SsResource ssResource)
        {
            _resources.Add(ssResource);
        }

        public List<SsResource> GetAll()
        {
            return _resources.ToList();
        }
    }
}