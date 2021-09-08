﻿using System.ComponentModel.DataAnnotations;
using System;

namespace NRCan.Datahub.Shared.EFCore
{
    public class Project_PBI_Workspace
    {
        [Key]
        public Guid Id { get; set; }

        public Datahub_Project Project { get; set; }
    }
}