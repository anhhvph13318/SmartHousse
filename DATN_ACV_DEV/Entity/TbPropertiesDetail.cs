﻿using System;
using System.Collections.Generic;

namespace DATN_ACV_DEV.Entity;

public partial class TbPropertiesDetail
{
    public Guid Id { get; set; }

    public Guid? PropertiesId { get; set; }

    public Guid? ProductId { get; set; }
}
