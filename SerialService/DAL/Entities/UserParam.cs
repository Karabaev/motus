namespace SerialService.DAL.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class UserParam : IBaseEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

        public bool Alike(IBaseEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}