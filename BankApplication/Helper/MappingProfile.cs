using AutoMapper;
using BankApplication.Dto;
using BankApplication.Models;

namespace BankApplication.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>();
            CreateMap<AccountDto, Account>();
            CreateMap<TransactionMoneyDto, TransactionMoney>();
            CreateMap<TransactionMoney, TransactionMoneyDto>();
        }
    }
}
