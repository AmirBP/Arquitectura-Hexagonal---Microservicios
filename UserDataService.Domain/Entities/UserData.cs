using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDataService.Domain.Entities
{
    public class UserData
    {
        public int Id { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Avatar { get; private set; }

        public UserData(int id, string firstName, string lastName, string avatar)
        {
            Id = id;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Avatar = avatar ?? throw new ArgumentNullException(nameof(avatar));
        }
    }
}
