﻿/**
 * Copyright (c) 2011 Adriano Carlos Verona
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Binboo.Core.Commands;
using Binboo.Core.Persistence;

namespace Binboo.Core.Plugins
{
    public abstract class AbstractBasePlugin
    {
        public IEnumerable<IBotCommand> Commands
        {
            get { return _commands; }
        }

        public ICommandResult ExecuteCommand(string commandName, IContext context)
        {
            var command = _commands.Where(cmd => StringComparer.InvariantCultureIgnoreCase.Compare(cmd.Id, commandName) == 0).Single();
            return command.Process(context);
        }

        protected void AddCommand(IStorageManager storageManager, IBotCommand command)
        {
            command.Storage = storageManager.StorageFor(command.Id);
            command.Initialize();

            _commands.Add(command);
        }

        private readonly IList<IBotCommand> _commands = new List<IBotCommand>();
        protected readonly ILog _log = LogManager.GetLogger(typeof(AbstractBasePlugin));
    }
}