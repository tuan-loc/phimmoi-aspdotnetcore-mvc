using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Exceptions.NotFound;
using PhimMoi.Domain.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.SharedLibrary.Helpers;

namespace PhimMoi.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWord;
        
        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWord = unitOfWork;
        }

        public async Task<Country> CreateAsync(Country country)
        {
            country.IdNumber = await _unitOfWord.CountryRepository.AnyAsync() ? await _unitOfWord.CountryRepository.MaxIdNumberAsync() + 1 : 1;
            country.Id = "country" + country.IdNumber.ToString();
            country.Name = country.Name.NormalizeString();
            country.NormalizeName = country.Name.RemoveMarks();

            _unitOfWord.CountryRepository.Create(country);
            await _unitOfWord.SaveAsync();

            return country;
        }

        public async Task DeleteAsync(string countryId)
        {
            Country? country = await _unitOfWord.CountryRepository.FirstOrDefaultAsync(c => c.Id == countryId);
            if (country == null) throw new CountryNotFoundException(countryId);
            _unitOfWord.CountryRepository.Delete(country);
            await _unitOfWord.SaveAsync();
        }

        public async Task<PagedList<Country>> GetAllAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWord.CountryRepository.GetAsync(pagingParameter);
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _unitOfWord.CountryRepository.GetAsync();
        }

        public async Task<Country?> GetByIdAsync(string id)
        {
            return await _unitOfWord.CountryRepository.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Country?> GetByNameAsync(string name)
        {
            name = name.RemoveMarks();
            return await _unitOfWord.CountryRepository.FirstOrDefaultAsync(x => x.NormalizeName == name);
        }

        public async Task<PagedList<Country>> SearchAsync(string? value, PagingParameter pagingParameter)
        {
            value = value?.RemoveMarks();
            PagedList<Country> countries = value != null ? await _unitOfWord.CountryRepository.GetAsync(pagingParameter: pagingParameter, c => (c.NormalizeName ?? "").Contains(value)) : await this.GetAllAsync(pagingParameter);
            return countries;
        }

        public async Task<Country> UpdateAsync(string countryId, Country country)
        {
            Country? countryToEdit = await _unitOfWord.CountryRepository.FirstOrDefaultAsync(c => c.Id == countryId);
            if (countryToEdit == null) throw new CountryNotFoundException(countryId);

            countryToEdit.Name = country.Name.NormalizeString();
            countryToEdit.NormalizeName = country.Name.RemoveMarks();
            countryToEdit.About = country.About;

            _unitOfWord.CountryRepository.Update(countryToEdit);
            await _unitOfWord.SaveAsync();

            return countryToEdit;
        }
    }
}