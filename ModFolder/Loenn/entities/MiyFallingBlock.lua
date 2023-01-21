local fakeTilesHelper = require("helpers.fake_tiles")

local miyFallingBlock = {}

miyFallingBlock.name = "SaladimHelper/MiyFallingBlock"
miyFallingBlock.placements = {
    name = "saladimHelper_falling_block",
    data = {
        tiletype = "3",
        climbFall = true,
        behind = false,
        width = 8,
        height = 8
    }
}

miyFallingBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", false)
miyFallingBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

function miyFallingBlock.depth(room, entity)
    return entity.behind and 5000 or 0
end

return miyFallingBlock