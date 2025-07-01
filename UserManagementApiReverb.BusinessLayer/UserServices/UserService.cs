using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;

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
}