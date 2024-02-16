#NoEnv
SendMode Input
SetTitleMatchMode, 2

VK_BROWSER_HOME := 0xAC
VK_BROWSER_BACK := 0xA6
VK_BROWSER_FORWARD := 0xA7
VK_BROWSER_FAVORITES := 0xAB

Browser_Home:: 
Send, !{Tab}
return

Browser_Favorites:: 
Send, {F5}
return

Browser_Forward:: 
Send, ^{Tab}
return

Browser_Back:: 
Send, ^+{Tab}
return