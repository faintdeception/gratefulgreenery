var boxes;
var hovering = false;

function newMap(mapId, location, zoom, mType) {
	//console.log($("#" + mapId));
	return new Microsoft.Maps.Map($("#"+mapId)[0],
        {
            credentials: "AuGiIj4rXiwF3vDgMFGCYrO4zXx0cZSFi7iJY0byEGiSWZUeylbeRkz0L7HPzm2C",
			center: location,
			zoom: zoom,
			mapTypeId: mType,
			enableSearchLogo: false,
			enableClickableLogo: false
			}
		);
}

function newPin(loc, folder, icon) {
	var pin = new Microsoft.Maps.Pushpin(loc);
	if (icon !== ""){ pin.setOptions({ icon: folder + icon }); }
	return pin;
}


function newLocation(lat, lng) {
	return new Microsoft.Maps.Location(lat, lng);
}


function getInfoBox(id) {
	return boxes["b" + id];
}

function hideAllBoxes() {
	var b;
	for (b in boxes) {
		if (boxes.hasOwnProperty(b)) {
			boxes[b].setOptions({ visible: false });
		}
	}
}

function showBox(bId) {
	hideAllBoxes();
	hovering = false;
	getInfoBox(bId).setOptions({ visible: true });
}

function hoverStart(bId) {
	var box = getInfoBox(bId);
	if (!box.getVisible()) {
		hovering = true;
		box.setOptions({ visible: true });
	}
}

function hoverEnd(bId) {
	if (hovering) {
		hovering = false;
		getInfoBox(bId).setOptions({ visible: false });
	}
}

function doPinStuff(p, map, iconFolder) {
	var loc = newLocation(p.Lat, p.Lng);
	var pin = newPin(loc, iconFolder, p.Icon);
	var iBox = new Microsoft.Maps.Infobox(loc, { title: p.Title, titleClickHandler: function () { window.location = p.Url; }, visible: false, height: 50, offset: new Microsoft.Maps.Point(0, 40) });
	Microsoft.Maps.Events.addHandler(pin, "click", function () { showBox(p.Id); });
	Microsoft.Maps.Events.addHandler(pin, "mouseover", function () { hoverStart(p.Id); });
	Microsoft.Maps.Events.addHandler(pin, "mouseout", function () { hoverEnd(p.Id); });
	map.entities.push(iBox);
	map.entities.push(pin);
	boxes["b" + p.Id] = iBox;
}

function addPins(pins, map, iconFolder) {
	boxes = [];
	for (pI = 0; pI < pins.length; pI++ ) {
		doPinStuff(pins[pI], map, iconFolder);
	}
}