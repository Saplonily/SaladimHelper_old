local fakeTilesHelper = require("helpers.fake_tiles")

local lightInvisibilityBlock = {}

lightInvisibilityBlock.name = "SaladimHelper/LightInvisibilityFloatBlock"
lightInvisibilityBlock.placements = {
    name = "saladimHelper_lightInvisibilityFloatBlock",
    data = {
        width = 8,
        height = 8,
        tile_type = "3",
        disableSpawnOffset = false,
        fear_player_light = true
    }
}

lightInvisibilityBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tile_type", false)
lightInvisibilityBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tile_type")

return lightInvisibilityBlock
