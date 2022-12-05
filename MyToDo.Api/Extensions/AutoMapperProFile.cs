using AutoMapper.Configuration;
using MyToDo.Api.Context;
using MyToDo.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToDo.Api.Extensions
{
    public class AutoMapperProFile : MapperConfigurationExpression
    {
        public AutoMapperProFile()
        {
            // 使得ToDo和ToDoDto能相互转化，执行该条语句之后，才能调用
            // var todo = Mapper.Map<ToDo>(ToDoDto);
            CreateMap<ToDo, ToDoDto>().ReverseMap();
            CreateMap<Memo, MemoDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
        
    }
}
