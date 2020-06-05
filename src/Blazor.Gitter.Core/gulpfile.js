var gulp = require("gulp"),
    sourcemaps = require("gulp-sourcemaps"),
    sass = require("gulp-sass"),
    es = require('event-stream'),
    rename = require("gulp-rename"),
    cleanCSS = require('gulp-clean-css');

gulp.task("sass", async function () {
    var client = gulp.src('content/css/blazor.gitter.scss')
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(cleanCSS())
        .pipe(rename("blazored.gitter.min.css"))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('../Blazor.Gitter.Client/wwwroot/css'));

    var server = gulp.src('content/css/blazor.gitter.scss')
        .pipe(sourcemaps.init())
        .pipe(sass())
        .pipe(cleanCSS())
        .pipe(rename("blazored.gitter.min.css"))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('../Blazor.Gitter.Server/wwwroot/css'));

    return es.concat(client, server);
});
