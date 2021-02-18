using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data.Entities.Models;

namespace CoreCodeCamp.Data
{
    public class CampProfile : Profile
    {

        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ForMember(c=> c.Venue, x=>x.MapFrom(a=>a.Location.VenueName))
                .ReverseMap()
                

                ;

            //this.CreateMap<CampModel, Camp>()
            //    .ForMember(a => a.Location.VenueName, b => b.MapFrom(c => c.Venue))
            //    .ForMember(a=>a.CampId, b=> b.Ignore())

            //    ;



        }
    }
}
