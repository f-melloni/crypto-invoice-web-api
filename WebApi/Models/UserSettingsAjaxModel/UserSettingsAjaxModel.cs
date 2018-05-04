using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.UserSettingsAjaxModel
{
    public class UserSettingsAjaxModel
    {
        [StringLength(200)]
        public string BTCXPUB { get; set; }

        [StringLength(200)]
        public string LTCXPUB { get; set; }

        [StringLength(200)]
        public string ETHAccount { get; set; }

        [StringLength(200)]
        public string XMRAddress { get; set; }

        [StringLength(200)]
        public string XMRPrivateViewKey { get; set; }

        [StringLength(200)]
        public string XMRPublicViewKey { get; set; }
    }
}
