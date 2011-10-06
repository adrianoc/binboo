Binboo is a skype bot that listens (in chats) for strings conforming to a specific format and then handle then
as commands. We use it as an interface to our issue track system.

Binboo can load "plugins" (any class that implements the interface Binboo.Core.Plugins.IPlugin) thorough MEF.

So far I have implemented one plugin (which motivated me to start binboo project) called "jira"; this plugin can add/query/update 
issues in a jira server.

I also have an experimental plugin for translation (which unfortunatelly was named as "dict") that uses Bing API to provide translation services
within skype chats.

To trigger a plugin Binboo scans all chat messages looking for a message starting with $ followed by the name of the plugin. So, to trigger the 
jira plugin one enters $jira followed by any parameters in a skype chat.