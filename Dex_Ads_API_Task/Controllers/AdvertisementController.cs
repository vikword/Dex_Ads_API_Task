using Dex_Ads_API_Task.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.OpenApi.Models;

namespace Dex_Ads_API_Task.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementController : ControllerBase
    {
        private AddDBContext _db;
        private int _maxAds = 2;
        public AdvertisementController(AddDBContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Получить список всех объявлений.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Advertisement>>> Get()
        {
            return await _db.Ads.ToListAsync();
        }

        /// <summary>
        /// Поиск объявления по всем полям.
        /// </summary> 
        /// <remarks>
        /// GET /Api/Advertisement/search-query  Type - string
        /// </remarks>
        [HttpGet("{searchQuery}")]
        public IEnumerable<Advertisement> Search(string searchQuery)
        {
            if (Guid.TryParse(searchQuery, out Guid id))
            {
                return _db.Ads.Where(x => x.Id == id || x.User == id);
            }
            else if (int.TryParse(searchQuery, out int num))
            {
                return _db.Ads.Where(x => x.Number == num);
            }
            else if (DateTime.TryParse(searchQuery, out DateTime date))
            {
                return _db.Ads.Where(x => x.Created.Date == date || x.ExpirationDate.Date == date);
            }

            return _db.Ads.Where(x => x.Text.Contains(searchQuery));
        }

        /// <summary>
        /// Добавить новое объявление.
        /// </summary>    
        /// <param name="advertisement">Объект типа Advertisemen</param>
        /// <param name="name">Имя пользователя для создания нового пользоватебя если он еще не существует</param>
        /// <response code="200">If the item is null</response> 
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Advertisement>> Create(Advertisement advertisement, string name)
        {
            if (advertisement == null)
            {
                return BadRequest("Объект отсутствует");
            }

            if (!_db.Users.Any(x => x.Id == Guid.Parse(Request.Cookies["id"])))
            {
                _db.Users.Add(new User() 
                { 
                    Id = Guid.Parse(Request.Cookies["id"]), 
                    Name = name,
                    Admin = !_db.Ads.Any()
                });
            }

            advertisement.Id = Guid.NewGuid();
            advertisement.User = Guid.Parse(Request.Cookies["id"]);
            advertisement.Number = _db.Ads.Count(a => a.User == advertisement.User);
            advertisement.Created = DateTime.Now;

            if (advertisement.Number >= _maxAds)
            {
                return BadRequest($"Пользователь не может добавлять более {_maxAds} объявлений") ;
            }

            _db.Ads.Add(advertisement);
            await _db.SaveChangesAsync();
            return Ok(advertisement);
        }

        /// <summary>
        /// Изменить объявление.
        /// </summary>
        /// <remarks>
        /// PUT /api/Advertisement
        /// </remarks>
        [HttpPut]
        public async Task<ActionResult<Advertisement>> ToChange(Advertisement advertisement)
        {
            if (advertisement == null)
            {
                return BadRequest();
            }
            if (!_db.Ads.Any(x => x.Id == advertisement.Id))
            {
                return NotFound();
            }

            if (advertisement.User != Guid.Parse(Request.Cookies["id"]))
            {
                return BadRequest("Вы не можете редактировать это объявление так как его добавил другой пользователь");
            }

            _db.Update(advertisement);
            await _db.SaveChangesAsync();
            return Ok(advertisement);
        }

        ///<summary>
        ///Удалить объявление.
        ///</summary>
        ///<remarks>
        /// DELETE /api/Advertisement/3fa85f64-5717-4562-b3fc-2c963f66afa6  Type Guid
        ///</remarks>
        ///<param name="id">id объявления Guid, пример - 3fa85f64-5717-4562-b3fc-2c963f66afa6</param>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Advertisement>> Delete(Guid id)
        {
            Advertisement advertisement = _db.Ads.FirstOrDefault(x => x.Id == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            if (advertisement.User != Guid.Parse(Request.Cookies["id"]))
            {
                return BadRequest("Вы не можете редактировать это объявление так как оно не ваше");
            }
            
            _db.Ads.Remove(advertisement);
            await _db.SaveChangesAsync();
            return Ok(advertisement);
        }
    }
}
