local fakeTilesHelper = require("helpers.fake_tiles")

local flagExitBlock = {}

flagExitBlock.name = "SaladimHelper/FlagExitBlock"
flagExitBlock.depth = -13000
flagExitBlock.placements = {
    name = "saladimHelper_flagExitBlock",
    data = {
        tileType = "3",
        playTransitionReveal = false,
        width = 8,
        height = 8,
        expected_flag = ""
    }
}

flagExitBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tileType", false, "tilesFg", {1.0, 1.0, 1.0, 0.7})
flagExitBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tileType")

return flagExitBlock