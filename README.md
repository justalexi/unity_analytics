## Small service to send analytics events from unity

Requires:
1. TextMesh Pro
2. Best HTTP/2 asset (https://assetstore.unity.com/packages/tools/network/best-http-2-155981)
3. More Effective Coroutines [FREE] (https://assetstore.unity.com/packages/tools/animation/more-effective-coroutines-free-54975)

If More Effective Coroutines [PRO] is available then the code can be simplified a bit.


Usage:
1. Play TestAnalyticsScene scene.
2. Write an address of a listening web service (for example, https://httpbin.org/post)
3. Press "Restart Service" button
4. Press any of the events buttons and wait (duration is set in TestAnalyticsService object)
