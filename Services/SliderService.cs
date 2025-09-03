using System.Collections.Generic;
using Verveo.DataAccess;
using Verveo.Entities;

namespace Verveo.Services
{
    public class SliderService
    {
        private readonly ISliderRepository _repo;

        public SliderService(ISliderRepository repo)
        {
            _repo = repo;
        }

        public List<Slider> GetAllSliders() => _repo.GetAll();
        public Slider GetSliderById(int id) => _repo.GetById(id);
        public void AddSlider(Slider slider) => _repo.Add(slider);
        public void UpdateSlider(Slider slider) => _repo.Update(slider);
        public void DeleteSlider(int id) => _repo.Delete(id);
    }
}
