using TradingCompany.DTO;

namespace TradingCompany.BLL.Interfaces
{
    public interface IAuthorizationManager
    {
        Employee Login(string login, string password);

        bool IsWarehouseManager(Employee employee);
    }
}