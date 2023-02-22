local fakeTilesHelper = require("helpers.fake_tiles")

local lightInvisibilityBlock = {}

lightInvisibilityBlock.name = "SaladimHelper/LightInvisibilityBlock"
lightInvisibilityBlock.placements = {
    name = "saladimHelper_lightInvisibilityBlock",
    data = {
        width = 8,
        height = 8,
        tile_type = "3",
        fear_player_light = true,
        light_radius_radio = 1.0
    }
}

lightInvisibilityBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tile_type", false)
lightInvisibilityBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tile_type")

return lightInvisibilityBlock
