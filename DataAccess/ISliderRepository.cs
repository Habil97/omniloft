using Verveo.Entities;
using System.Collections.Generic;

public interface ISliderRepository
{
    List<Slider> GetAll();
    Slider GetById(int id);
    void Add(Slider slider);
    void Update(Slider slider);
    void Delete(int id);
}
