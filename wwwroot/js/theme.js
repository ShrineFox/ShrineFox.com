var accent = '51,186,54';
var text = '139,148,158';
var bg = '01,04,09';
var rgb = 0;

document.addEventListener("DOMContentLoaded", () => {
	SetColor();
	SetDarkMode();
	setInterval(function () {
		RotateRGB();
	}, 2000);
});

function SetColor() {
	var color = getCookie("color");
	HideColorPicker();

	switch (color) {
		case 'red':
			accent = '180,41,41';
			break;
		case 'orange':
			accent = '227,128,75';
			break;
		case 'yellow':
			accent = '191,175,50';
			break;
		case 'green':
			accent = '51,186,54';
			break;
		case 'blue':
			accent = '64,137,206';
			break;
		case 'violet':
			accent = '156,87,188';
			break;
		case 'pink':
			accent = '234,142,226';
			break;
		case 'magenta':
			accent = '195,77,127';
			break;
		case 'gamer':
			accent = '169,71,71';
			break;
		case 'custom':
			ShowColorPicker();
			/* Load color values from cookie */
			if (getCookie("color_custom") == "")
			{
				setCookie("color_custom", "50,50,50", 999);
			}
			accent = getCookie("color_custom");
			break;
		default:
			accent = '51,186,54';
			break;
	}

	/* Override CSS color values */
	document.documentElement.style.setProperty('--accent', accent);
}

function RotateRGB() {
	var color = getCookie("color");
	if (color == "gamer")
	{
		switch (rgb)
		{
			case 0:
				accent = '169,71,71';
				break;
			case 1:
				accent = '227,168,95';
				break;
			case 2:
				accent = '191,175,80';
				break;
			case 3:
				accent = '68,163,69';
				break;
			case 4:
				accent = '92,149,199';
				break;
			case 5:
				accent = '156,87,188';
				break;
			default:
				accent = '169,71,71';
				break;
		}
		rgb++;
		/* Override CSS color values */
		document.documentElement.style.setProperty('--accent', accent);
	}
	if (rgb > 5) { rgb = 0 }
}

function DarkModeSelect() {
	/* Update light/dark mode cookie */
	var toggle = document.getElementById("darkToggle");
	if (toggle.checked == true) {
		setCookie("darkmode", "on", 999);
	} else {
		setCookie("darkmode", "off", 999);
	}

	SetDarkMode();
}

function SetDarkMode() {
	var darkmode = getCookie("darkmode");
	var toggle = document.getElementById("darkToggle");

	switch (darkmode) {
		case 'on':
			toggle.checked = true;
			document.documentElement.className = 'fd_dark';
			text = '139,148,158';
			bg = '01,04,09';
			break;
		case 'off':
			toggle.checked = false;
			document.documentElement.className = 'fd_light';
			text = '01,04,09';
			bg = '225,225,225';
			break;
		default:
			toggle.checked = true;
			document.documentElement.className = 'fd_dark';
			text = '139,148,158';
			bg = '01,04,09';
			break;
	}

	/* Override CSS values */
	document.documentElement.style.setProperty('--text', text);
	document.documentElement.style.setProperty('--bg', bg);
}

function ColorSelect() {
	/* Update color scheme cookie */
	var color = document.getElementById("color").value.toLowerCase();
	setCookie("color", color, 999);

	SetColor();
}

function updateAccent(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_custom", rgb, 999);
	SetColor();
}

function updateColorPicker() {
	accentstring = 'rgba(' + accent + ',1);'
	document.querySelector('#customaccent').jscolor.fromString(accentstring);
}

function getCookie(cname) {
	var name = cname + "=";
	var decodedCookie = decodeURIComponent(document.cookie);
	var ca = decodedCookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function setCookie(cname, val, exdays) {
	var exdate = new Date();
	exdate.setDate(exdate.getDate() + exdays);
	var c_value = escape(val) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
	document.cookie = cname + "=" + c_value + ";path=/";
}

function selectElement(id, valueToSelect) {
	let element = document.getElementById(id);
	element.value = valueToSelect;
}

function ShowColorPicker() {
	var c = document.getElementById('colorpicker');
	c.setAttribute("style", "display: initial;");
}

function HideColorPicker() {
	var c = document.getElementById('colorpicker');
	c.setAttribute("style", "display: none;");
}

(function () {

	let accent = '51,186,54';
	let text = '139,148,158';
	let bg = '01,04,09';
	let rgb = 0;
	let rgbTimer = null;

	// PUBLIC ENTRY POINT
	window.initTheme = function () {
		SetColor();
		SetDarkMode();
		StartRGBRotation();
	};

	function StartRGBRotation() {
		// Prevent multiple intervals on navigation
		if (rgbTimer !== null) return;

		rgbTimer = setInterval(() => {
			RotateRGB();
		}, 2000);
	}

	function SetColor() {
		const color = getCookie("color");
		HideColorPicker();

		switch (color) {
			case 'red': accent = '180,41,41'; break;
			case 'orange': accent = '227,128,75'; break;
			case 'yellow': accent = '191,175,50'; break;
			case 'green': accent = '51,186,54'; break;
			case 'blue': accent = '64,137,206'; break;
			case 'violet': accent = '156,87,188'; break;
			case 'pink': accent = '234,142,226'; break;
			case 'magenta': accent = '195,77,127'; break;
			case 'gamer': accent = '169,71,71'; break;
			case 'custom':
				ShowColorPicker();
				if (getCookie("color_custom") === "") {
					setCookie("color_custom", "50,50,50", 999);
				}
				accent = getCookie("color_custom");
				break;
			default:
				accent = '51,186,54';
				break;
		}

		document.documentElement.style.setProperty('--accent', accent);
	}

	function RotateRGB() {
		if (getCookie("color") !== "gamer") return;

		const colors = [
			'169,71,71',
			'227,168,95',
			'191,175,80',
			'68,163,69',
			'92,149,199',
			'156,87,188'
		];

		accent = colors[rgb] ?? colors[0];
		rgb = (rgb + 1) % colors.length;

		document.documentElement.style.setProperty('--accent', accent);
	}

	// Exposed for UI bindings
	window.DarkModeSelect = function () {
		const toggle = document.getElementById("darkToggle");
		if (!toggle) return;

		setCookie("darkmode", toggle.checked ? "on" : "off", 999);
		SetDarkMode();
	};

	function SetDarkMode() {
		const darkmode = getCookie("darkmode");
		const toggle = document.getElementById("darkToggle");

		if (darkmode === 'off') {
			document.documentElement.className = 'fd_light';
			text = '01,04,09';
			bg = '225,225,225';
			if (toggle) toggle.checked = false;
		} else {
			document.documentElement.className = 'fd_dark';
			text = '139,148,158';
			bg = '01,04,09';
			if (toggle) toggle.checked = true;
		}

		document.documentElement.style.setProperty('--text', text);
		document.documentElement.style.setProperty('--bg', bg);
	}

	window.ColorSelect = function () {
		const el = document.getElementById("color");
		if (!el) return;

		setCookie("color", el.value.toLowerCase(), 999);
		SetColor();
	};

	window.updateAccent = function (picker) {
		const rgbVal = picker.toRGBString()
			.replace("rgb(", "")
			.replace(")", "");

		setCookie("color_custom", rgbVal, 999);
		SetColor();
	};

	window.updateColorPicker = function () {
		const picker = document.querySelector('#customaccent');
		if (!picker || !picker.jscolor) return;

		picker.jscolor.fromString(`rgba(${accent},1)`);
	};

	function ShowColorPicker() {
		const c = document.getElementById('colorpicker');
		if (c) c.style.display = 'initial';
	}

	function HideColorPicker() {
		const c = document.getElementById('colorpicker');
		if (c) c.style.display = 'none';
	}

	function getCookie(cname) {
		const name = cname + "=";
		const decodedCookie = decodeURIComponent(document.cookie);
		const ca = decodedCookie.split(';');

		for (let c of ca) {
			c = c.trim();
			if (c.indexOf(name) === 0) {
				return c.substring(name.length);
			}
		}
		return "";
	}

	function setCookie(cname, val, exdays) {
		const exdate = new Date();
		exdate.setDate(exdate.getDate() + exdays);
		document.cookie =
			`${cname}=${encodeURIComponent(val)}; expires=${exdate.toUTCString()}; path=/`;
	}

})();