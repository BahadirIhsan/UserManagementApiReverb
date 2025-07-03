using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.UserServices;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IUserMapper _mapper;
    
    public UserService(AppDbContext db,  IUserMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }


    public async Task<UserResponse> GetUserAsyncById(Guid UserId)
    {
        var user = await _db.Users.FindAsync(UserId); // FindAsync() ifadesi sadece pk olan ifadeleri bulur onun 
        // haricindeki işlemler için farklı bir şekilde sorgu yapmak gerekir.
        if (user == null)
        {
            return null;
        }
        return _mapper.MapUserToUserResponse(user);
    }

    public async Task<UserResponse> GetUserAsyncByEmail(string Email)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == Email); // pk bir variable 
        // olmadığı için FindAsync kullanamıyoruz bunun yerine FirstOrDefaultAsync kullanıyoruz. Çünkü FindAsync sadece pk variable'ları arar diğerlerini aramaz.
        // burada ek olarak AsNoTracking diye bir yapı kullandık bunun sayesinde memory'den tasarruf sağlarız ve ek olarak işleyiş şekli: 
        // çektiği verileri tutmaz yani saklamaz yani bir önceki ve sonraki çektiği veriler aynı veriler olsa dahi aynı değilmiş gibi olur sadece bir copy gibi 
        // çeker verileri bunu sadece verileri çekip göstermek için kullanabiliriz. Update işlemlerinde vesaire kullanamayız çünkü verinin kendisini getirmez.
        // bu yapıyı sadece daha iyi anlaşılan bir örnek olsun diye verdim. asıl olay ise changeTracker içine kaydedilip kaydedilmeme mevzusu
        // bu ifade kullanılarak çekilen veriler changeTracker'a kaydedilmez. Kopya demek yerine "izlenmeyen taze nesne" demek doğru olur. 
        if (user == null)
        {
            return null;
        }
        return _mapper.MapUserToUserResponse(user);
    }

    public async Task<UserResponse> GetUserAsyncByUsername(string UserName)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == UserName);
        if (user == null)
        {
            return null;
        }
        return _mapper.MapUserToUserResponse(user);
    }
    
    public async Task<UserResponse> GetUserByEmailOrUsernameAsync(string? Email, string? Username)
    {
        bool hasUserName = !string.IsNullOrWhiteSpace(Username);
        bool hasEmail = !string.IsNullOrWhiteSpace(Email);

        if (hasUserName && hasEmail)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == Email && u.UserName == Username);
            return _mapper.MapUserToUserResponse(user);
        }
        else if (hasUserName && !hasEmail)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == Username);
            return _mapper.MapUserToUserResponse(user);
        }
        else if (!hasUserName && hasEmail)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == Email);
            return _mapper.MapUserToUserResponse(user);
        }
        else
        {
            return null;
        }
    }
    
    public async Task<PagedResult<UserResponse>> GetAllUsersPaginationAsync(Paging paging, Sorting sorting)
    {
        IQueryable<User> query = _db.Users.AsNoTracking();
        query = sorting.sortDir == "desc" ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt);
        
        int totalCount = await query.CountAsync();
        
        int skip = (paging.Page - 1) * paging.PageSize;
        query = query.Skip(skip).Take(paging.PageSize);
        
        List<User> users = await query.ToListAsync();
        var items = users.Select(u => _mapper.MapUserToUserResponse(u));

        return new PagedResult<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = paging.Page,
            PageSize = paging.PageSize
        };

    }

    public async Task<UserResponse> CreateUserAsync(UserRequestRegister req)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email))
        {
            throw new ArgumentException("Email already exists");
        }

        if (await _db.Users.AnyAsync(u => u.UserName == req.Username))
        {
            throw new ArgumentException("UserName already exists");
        }

        string hash = BCrypt.Net.BCrypt.HashPassword(req.Password);
        
        var user = _mapper.MapFromRegisterRequest(req,hash);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        
        return _mapper.MapUserToUserResponse(user);
    }

    public async Task<UserResponse> UpdateUserAsync(UserRequestUpdate req)
    {
        var user = await _db.Users.FindAsync(req.Id);
        if (user == null)
        {
            return null;
        }

        if (req.UserName != null && req.UserName != user.UserName)
        {
            bool userNameTaken = await _db.Users.AnyAsync(u => u.UserName == req.UserName && u.UserId != req.Id);
            if (userNameTaken)
            {
                throw new ArgumentException("UserName already exists");
            }
            user.UserName = req.UserName;
        }

        if (req.Email != null && req.Email != user.Email)
        {
            bool emailTaken = await _db.Users.AnyAsync(u => u.Email == req.Email && u.UserId == req.Id);
            if (emailTaken)
            {
                throw new ArgumentException("Email already exists");
            }
            user.Email = req.Email;
        }
        
        if(req.FirstName != null) user.FirstName = req.FirstName;
        if (req.LastName != null) user.LastName = req.LastName;
        if(req.PhoneNumber != null)  user.PhoneNumber = req.PhoneNumber;
        if (req.Address != null) user.Address = req.Address;
        if (req.Birthday.HasValue) user.Birthday = req.Birthday;
        
        await _db.SaveChangesAsync();
        return _mapper.MapUserToUserResponse(user);
        
    }

    public async Task<bool> DeleteUserAsync(Guid UserId)
    {
        var user = await _db.Users.FindAsync(UserId);
        if (user == null) return false;
        
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
        
    }
    
}