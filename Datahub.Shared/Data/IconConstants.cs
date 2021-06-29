﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRCan.Datahub.Shared.Data
{
  public class Icon
  {
    public string Name { get; set; }
    public string Color { get; set; }

    public static readonly Icon HOME = new()
    {
      Name = "fad fa-home",
      Color = "blue",
    };

    public static readonly Icon STORAGE = new()
    {
      Name = "fad fa-hdd",
      Color = "indigo",
    };

    public static readonly Icon RESOURCES = new()
    {
      Name = "fad fa-project-diagram",
      Color = "purple",
    };

    public static readonly Icon DATASETS = new()
    {
      Name = "fad fa-cabinet-filing",
      Color = "pink",
    };

    public static readonly Icon POWERBI = new()
    {
      Name = "fad fa-chart-bar",
      Color = "blue",
    };

    public static readonly Icon ADMIN = new()
    {
      Name = "fad fa-user-cog",
      Color = "blue",
    };

    public static readonly Icon DATAENTRY = new()
    {
      Name = "fad fa-keyboard",
      Color = "blue",
    };
  }
}
