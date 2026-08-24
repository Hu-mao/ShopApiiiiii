using System;
using System.Collections.Generic;
using System.Text;
namespace Shop.Application.Interfaces.Services;

public interface IQueueService
{
    Task PublishAsync<T>(string queue, T message);
}