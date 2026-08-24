using System;
using System.Collections.Generic;
using System.Text;
namespace Shop.Infrastructure.Configuration;

public sealed class RabbitMqSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}