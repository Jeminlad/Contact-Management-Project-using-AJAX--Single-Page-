using ContactProject.Models;

namespace ContactProject.Repositories.Interface;

public interface IUserInterface
{
    Task<int> Register(t_User user);
    Task<t_User> Login(vm_Login user);    // Task<t_User?> GetOne(string userid);
    // Task<List<t_User>> GetAll();
    // Task<int> Update(t_User user);
    // Task<int> Delete(string userid);    
}