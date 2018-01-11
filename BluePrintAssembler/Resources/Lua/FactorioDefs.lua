-- some factorio defines
defines = {}
defines.direction = {}
defines.direction.north = 1
defines.direction.east = 2
defines.direction.south = 3
defines.direction.west = 4
defines.difficulty_settings = {}
defines.difficulty_settings.recipe_difficulty = {}
defines.difficulty_settings.technology_difficulty = {}
defines.difficulty_settings.recipe_difficulty.normal = 1
defines.difficulty_settings.technology_difficulty.normal = 1
mods = {}

-- initialize settings
settings = {}
settings.startup = {}

-- provide required functions
function log(...) end
function module(...) end

serpent = {}
function serpent.dump(...) return "" end

-- load and override util module
require "util"
util = {}
util.table = {}
util.table.deepcopy = table.deepcopy
util.multiplystripes = multiplystripes
util.by_pixel = by_pixel
util.format_number = format_number
util.increment = increment

