using System.Collections.Generic;
using System.Linq;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public class EfSliderRepository : ISliderRepository
    {
        private readonly VerveoDbContext _db;

        public EfSliderRepository(VerveoDbContext db)
        {
            _db = db;
        }

        public List<Slider> GetAll() => _db.Sliders.ToList();

        public Slider GetById(int id) => _db.Sliders.FirstOrDefault(s => s.Id == id);

        public void Add(Slider slider)
        {
            _db.Sliders.Add(slider);
            _db.SaveChanges();
        }

        public void Update(Slider slider)
        {
            _db.Sliders.Update(slider);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var slider = _db.Sliders.FirstOrDefault(s => s.Id == id);
            if (slider != null)
            {
                _db.Sliders.Remove(slider);
                _db.SaveChanges();
            }
        }
    }
}
