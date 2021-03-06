<?php
/**
 * The template for displaying the footer.
 *
 * Contains the closing of the #content div and all content after.
 *
 * @link https://developer.wordpress.org/themes/template-files-section/partial-and-miscellaneous-template-files/#footer-php
 *
 * @package Primer
 * @since   1.0.0
 */

?>

		</div><!-- #content -->


	</div><!-- #page -->

	
<footer class="footer text-muted">
    <div class="container">
        <div class="columns">
            <div class="column col-6">
                &copy; ShrineFox 2020 - 2021
                <script>
                    document.addEventListener("DOMContentLoaded", function (event) {
                        var controller = new YTV('YourPlayerID', {
                            channelId: 'UCrB3t1zAQPwAeWtI8RZIOvQ',
                            playlist: 'PLU6By7bu-RSsXcvqRkDdh3R4OGva8_No4',
                            responsive: true
                        });
                    });
                </script>
                <br><a href="https://ko-fi.com/shrinefox"><i class="fa fa-coffee"></i> Support</a> | <a href="https://bit.ly/shrinefox"><i class="fab fa-trello"></i> Progress</a> | <a href="https://shrinefox.com/labs"><i class="fa fa-flask"></i> Labs</a>
                <br><a href="https://shrinefox.com/privacy"><i class="fa fa-user-secret"></i> Privacy Policy</a> | <a href="https://shrinefox.com/terms"><i class="fas fa-thumbs-up"></i> Terms & Conditions</a> | <a href="https://shrinefox.com/legal"><i class="fas fa-balance-scale-left"></i> Legal Disclaimer</a>
                <br><span class="g-ytsubscribe" data-channel="ShrineFox" data-layout="default" data-count="default"></span>
            </div>
            <div class="column col-6 form-group">
                <!--Theme Select-->
                <select id="color" name="color" class="form-select" onchange="ColorSelect()">
                    <option value="">Select a Theme</option>
                    <option value="Red">Red</option>
                    <option value="Orange">Orange</option>
                    <option value="Yellow">Yellow</option>
                    <option value="Green">Green</option>
                    <option value="Blue">Blue</option>
                    <option value="Violet">Violet</option>
                    <option value="Gamer">Gamer</option>
                    <option value="Custom">Custom</option>
                </select>
                <!--Color Picker-->
                <script>
                    jscolor.presets.default = {
                        format: 'rgb', previewSize: 20, paletteCols: 1,
                        backgroundColor: 'rgb(var(--bg))', borderColor: 'rgb(var(--bg))',
                        padding: 5, width: 100, height: 100, mode: 'HVS',
                        controlBorderColor: 'rgb(var(--bg))', sliderSize: 8, shadow: false
                    };
                </script>
                <span class="column col-6 form-group">
                    <label class="form-switch">
                        <input type="checkbox" id="darkToggle" onclick="DarkModeSelect()">
                        <i class="form-icon"></i> Dark Mode
                    </label>
                </span>
                <span class="column col-6 form-group" id="colorpicker" style="display:none;">
                    <button id="customaccent" data-jscolor="{onChange: 'updateAccent(this)',value:'rgb(var(--accent))', alpha:1}">Accent</button>
                </span>
            </div>
        </div>
        <div class="ad">
            <!--AdSense-->
            <script async="" src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
            <ins class="adsbygoogle" style="display:block" data-ad-format="fluid" data-ad-layout-key="-hx+4+s-4m+70" data-ad-client="ca-pub-9519592525056753" data-ad-slot="8263745709"></ins>
            <script>(adsbygoogle = window.adsbygoogle || []).push({});</script>
        </div>
        <!--Donate-->
        <script src='https://storage.ko-fi.com/cdn/scripts/overlay-widget.js'></script>
        <script>
            kofiWidgetOverlay.draw('shrinefox', {
                'type': 'floating-chat',
                'floating-chat.donateButton.text': '',
                'floating-chat.donateButton.background-color': 'rgba(var(--accent), 1)',
                'floating-chat.donateButton.text-color': 'rgba(var(--text), 1)'
            });
        </script>
    </div>
</footer>


</body>

</html>
