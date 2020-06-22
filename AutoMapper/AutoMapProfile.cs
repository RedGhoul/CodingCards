using AutoMapper;
using CodingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.AutoMapper
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<Card, CardDTO>();
            CreateMap<CardDTO, Card>();
        }

    }
}
