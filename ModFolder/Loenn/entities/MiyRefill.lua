local refill = {}

refill.name = "SaladimHelper/MiyRefill"
refill.depth = -100
refill.placements = {
    {
        name = "saladimHelper_miyRefill_oneDash",
        data = {
            oneUse = false,
            twoDash = false
        }
    },
    {
        name = "saladimHelper_miyRefill_twoDash",
        data = {
            oneUse = false,
            twoDash = true
        }
    }
}

function refill.texture(room, entity)
    return entity.twoDash and "objects/refillTwo/idle00" or "objects/refill/idle00"
end

return refill
