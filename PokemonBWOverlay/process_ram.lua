local last_check = 0
local bnd,br,bxr=bit.band,bit.bor,bit.bxor
local rshift, lshift=bit.rshift, bit.lshift
BlockA = {1,1,1,1,1,1, 2,2,3,4,3,4, 2,2,3,4,3,4, 2,2,3,4,3,4}



function mult32(a,b)
	local c=rshift(a,16)
	local d=a%0x10000
	local e=rshift(b,16)
	local f=b%0x10000
	local g=(c*f+d*e)%0x10000
	local h=d*f
	local i=g*0x10000+h
	return i
end

function getbits(a,b,d)
	return rshift(a,b)%lshift(1,d)
end

function gettop(a)
	return(rshift(a,16))
end

function getPidAddr()
	if mode == 5 then
		return 0x02259DD8
	elseif mode == 4 then
		return 0x0226B7B4 + 0xDC*(submode-1)
	elseif mode == 3 then
		return 0x0226C274 + 0xDC*(submode-1)
	elseif mode == 2 then
		return 0x0226ACF4 + 0xDC*(submode-1)
	else -- mode 1
		return 0x022349B4 + 0xDC*(submode-1) 
	end
end
 
function fn()
    current_time = os.time()
	if current_time - last_check > 2 then
		mode = 1
		checkdata()
    end
end

function checkdata()
	pidAddr = 0x022349B4
	pid = memory.readdword(pidAddr)
	checksum = memory.readword(pidAddr + 6)
	shiftvalue = (rshift((bnd(pid,0x3E000)),0xD)) % 24
	
	BlockAoff = (BlockA[shiftvalue + 1] - 1) * 32
	prng = checksum
	for i = 1, BlockA[shiftvalue + 1] - 1 do
		prng = mult32(prng,0x5F748241) + 0xCBA72510
	end
	
	prng = mult32(prng,0x41C64E6D) + 0x6073
	pokemonID = bxr(memory.readword(pidAddr + BlockAoff + 8), gettop(prng))
	if pokemonID > 651 then
		pokemonID = -1
	end
	pidAddr = 0x022A7E0C
	pid = memory.readdword(pidAddr)
	checksum = memory.readword(pidAddr + 6)
	prng = checksum
	prng = mult32(prng,0x022A7E0C) + 0x6073
	value1 = bxr(memory.readword(0x022A7E0C), gettop(prng))

	pidAddr = 0x022A7E68
	pid = memory.readdword(pidAddr)
	checksum = memory.readword(pidAddr + 6)
	prng = checksum
	prng = mult32(prng,0x022A7E68) + 0x6073
	value2 = bxr(memory.readword(0x022A7E68), gettop(prng))

	pidAddr = 0x022A7EC4
	pid = memory.readdword(pidAddr)
	checksum = memory.readword(pidAddr + 6)
	prng = checksum
	prng = mult32(prng,0x022A7EC4) + 0x6073
	value3 = bxr(memory.readword(0x022A7EC4), gettop(prng))

	pidAddr = 0x02269838
	pid = memory.readdword(pidAddr)
	checksum = memory.readword(pidAddr + 6)
	prng = checksum
	prng = mult32(prng,0x02269838) + 0x6073
	value4 = bxr(memory.readword(0x02269838), gettop(prng))
	
	test = '{ "value1": ' .. value1 .. ', "value2": ' .. value2 .. ', "value3": ' .. value3 .. ', "value4": ' .. value4 .. ', "party1": ' .. pokemonID ..' }'
	file = io.open("output", "w")
	file:write(test)
	file:close()

	last_check = current_time
end

gui.register(fn)
