var accent = '51,186,54';
var text = '139,148,158';
var bg = '01,04,09';
var rgb = 0;

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
			accent = '255,0,96';
			break;
		case 'pride':
			accent = '255,128,216';
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
                accent = '180,41,41';
				break;
			case 1:
                accent = '227,128,75';
				break;
			case 2:
                accent = '191,175,50';
				break;
			case 3:
                accent = '51,186,54';
				break;
			case 4:
                accent = '64,137,206';
				break;
			case 5:
				accent = '156,87,188';
				break;
            case 6:
                accent = '255,0,96';
                break;
            case 7:
                accent = '255,128,216';
                break;
			default:
                accent = '180,41,41';
				break;
		}
		rgb++;
		/* Override CSS color values */
		document.documentElement.style.setProperty('--accent', accent);
	}
    if (rgb > 7) { rgb = 0 }
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

(() => {
    const DEFAULT_COLORS = {
        accent: '51,186,54',
        darkText: '139,148,158',
        darkBackground: '01,04,09',
        lightText: '01,04,09',
        lightBackground: '225,225,225'
    };

    const THEME_COLORS = {
        red: '180,41,41',
        orange: '227,128,75',
        yellow: '191,175,50',
        green: '51,186,54',
        blue: '64,137,206',
        violet: '156,87,188',
        pink: '255,0,96',
        pride: '255,128,216',
        gamer: '169,71,71'
    };

    const GAMER_COLORS = [
        '180,41,41',
        '227,128,75',
        '191,175,50',
        '51,186,54',
        '64,137,206',
        '156,87,188',
        '255,0,96',
        '255,128,216'
    ];

    let accent = DEFAULT_COLORS.accent;
    let text = DEFAULT_COLORS.darkText;
    let background = DEFAULT_COLORS.darkBackground;
    let gamerColorIndex = 0;
    let rgbTimer = null;
    let syncingPickers = false;

    window.initTheme = function () {
        restoreThemeSelection();
        applyTheme();
        startGamerRotation();
    };

    window.DarkModeSelect = function () {
        const toggle = document.getElementById('darkToggle');
        if (!toggle) return;

        setCookie('darkmode', toggle.checked ? 'on' : 'off', 999);
        applyTheme();
    };

    window.ColorSelect = function () {
        const selector = document.getElementById('color');
        if (!selector) return;

        setCookie('color', selector.value.toLowerCase(), 999);
        applyTheme();
    };

    window.updateAccent = function (picker) {
        if (syncingPickers) return;

        setCookie('color_custom', getPickerRgbValue(picker), 999);
        applyTheme();
    };

    window.updateText = function (picker) {
        if (syncingPickers) return;

        text = getPickerRgbValue(picker);
        setCookie('text_custom', text, 999);
        setCssVariable('--text', text);
    };

    window.updateBackground = function (picker) {
        if (syncingPickers) return;

        background = getPickerRgbValue(picker);
        setCookie('bg_custom', background, 999);
        setCssVariable('--bg', background);
    };

    window.updateColorPicker = syncPickers;

    function restoreThemeSelection() {
        const savedColor = getCookie('color');
        const selector = document.getElementById('color');
        if (!selector || !savedColor) return;

        const savedOption = Array.from(selector.options)
            .find(option => option.value.toLowerCase() === savedColor.toLowerCase());

        selector.value = savedOption?.value ?? '';
    }

    function applyTheme() {
        const color = getCookie('color');

        setAccentColor(color);
        setTextAndBackgroundColors(color);
        setHeaderImageStyle(color === 'pride');
        setGamerTransitionStyle(color === 'gamer');
        syncPickers();
    }

    function setAccentColor(color) {
        showOrHideCustomPicker(color === 'custom');

        if (color === 'custom') {
            if (!getCookie('color_custom')) {
                setCookie('color_custom', '50,50,50', 999);
            }
            accent = getCookie('color_custom');
        } else {
            accent = THEME_COLORS[color] ?? DEFAULT_COLORS.accent;
        }

        setCssVariable('--accent', accent);
    }

    function setTextAndBackgroundColors(color) {
        const darkMode = getCookie('darkmode');
        const toggle = document.getElementById('darkToggle');

        if (color === 'pride') {
            text = '0,181,246';
            if (darkMode === 'off') {
                document.documentElement.className = 'fd_light';
                background = DEFAULT_COLORS.lightBackground;
                if (toggle) toggle.checked = false;
            } else {
                document.documentElement.className = 'fd_dark';
                background = DEFAULT_COLORS.darkBackground;
                if (toggle) toggle.checked = true;
            }
        } else if (darkMode === 'off') {
            document.documentElement.className = 'fd_light';
            text = DEFAULT_COLORS.lightText;
            background = DEFAULT_COLORS.lightBackground;
            if (toggle) toggle.checked = false;
        } else {
            document.documentElement.className = 'fd_dark';
            text = DEFAULT_COLORS.darkText;
            background = DEFAULT_COLORS.darkBackground;
            if (toggle) toggle.checked = true;
        }

        if (color === 'custom') {
            text = getCookie('text_custom') || text;
            background = getCookie('bg_custom') || background;
        }

        setCssVariable('--text', text);
        setCssVariable('--bg', background);
    }

    function setHeaderImageStyle(isPride) {
        const headerImage = document.querySelector('.header-img');
        if (!headerImage) return;

        headerImage.style.mixBlendMode = isPride ? 'overlay' : 'luminosity';
        headerImage.style.opacity = isPride ? '1' : '0.5';
    }

    function setGamerTransitionStyle(isGamer) {
        if (!document.body) return;

        for (const element of document.body.querySelectorAll('*')) {
            element.style.transition = isGamer
                ? 'color 3s, background 3s, background-color 3s, border-color 3s, fill 3s, box-shadow 3s'
                : 'revert-rule';
        }
    }

    function startGamerRotation() {
        if (rgbTimer !== null) return;

        rgbTimer = setInterval(() => {
            if (getCookie('color') !== 'gamer') return;

            accent = GAMER_COLORS[gamerColorIndex];
            gamerColorIndex = (gamerColorIndex + 1) % GAMER_COLORS.length;
            setCssVariable('--accent', accent);
        }, 2000);
    }

    function syncPickers() {
        const pickers = [
            ['#customaccent', accent],
            ['#customtext', text],
            ['#custombg', background]
        ];

        syncingPickers = true;
        try {
            for (const [selector, value] of pickers) {
                const picker = document.querySelector(selector);
                if (picker?.jscolor) {
                    picker.jscolor.fromString(`rgba(${value},1)`);
                }
            }
        } finally {
            syncingPickers = false;
        }
    }

    function showOrHideCustomPicker(show) {
        const picker = document.getElementById('colorpicker');
        if (picker) picker.style.display = show ? 'initial' : 'none';
    }

    function getPickerRgbValue(picker) {
        return picker.toRGBString()
            .replace('rgb(', '')
            .replace(')', '');
    }

    function setCssVariable(name, value) {
        document.documentElement.style.setProperty(name, value);
    }

    function getCookie(name) {
        const prefix = `${name}=`;
        const cookie = document.cookie
            .split('; ')
            .find(value => value.startsWith(prefix));

        return cookie ? decodeURIComponent(cookie.substring(prefix.length)) : '';
    }

    function setCookie(name, value, days) {
        const expiration = new Date();
        expiration.setDate(expiration.getDate() + days);
        document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expiration.toUTCString()}; path=/`;
    }

    const initializeThemeOnLoad = () => window.initTheme?.();
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeThemeOnLoad, { once: true });
    } else {
        initializeThemeOnLoad();
    }
})();
