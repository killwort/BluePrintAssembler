-- internal functions
function ___BPA___pkgpath__add___(item)
	package.path = package.path .. ';' .. item .. '\\?.lua'
end
function ___BPA___pkgpath__set___(item)
	package.path = item .. '\\?.lua'
end
