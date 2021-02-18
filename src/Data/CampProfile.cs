using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data.Entities.Models;
using CoreCodeCamp.Models;
using CoreCodeCamp.Models.Models;

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

            this.CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(l => l.Camp, opt => opt.Ignore())
                .ForMember(l => l.Speaker, opt => opt.Ignore());
            ;
            
            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();

            //this.CreateMap<CampModel, Camp>()
            //    .ForMember(a => a.Location.VenueName, b => b.MapFrom(c => c.Venue))
            //    .ForMember(a=>a.CampId, b=> b.Ignore())

            //    ;



        }
    }
}
