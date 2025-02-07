using CantineAPI.Models;

namespace CantineAPI.Repositories
{
    public interface IRepasRepository
    {
        List<Repas> GetRepasByIds(List<int> repasIds);
        public IEnumerable<Repas> GetAll();
        public void Add(Repas repas);
        public void Remove(int id);
    }
}
