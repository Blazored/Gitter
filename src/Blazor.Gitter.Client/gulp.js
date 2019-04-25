/// <binding ProjectOpened='watch:styles' />
"use strict";

const gulp = require("gulp"),
    rimraf = require("rimraf"),
    cssmin = require("gulp-cssmin"),
    rename = require("gulp-rename"),
    sass = require("gulp-sass");

const paths = {
    webroot: "./wwwroot/",
    styles: "./Styles/"
};

paths.concatCssDest = paths.webroot + "css/";

gulp.task("clean:css", done => rimraf(paths.concatCssDest + "blazored-gitter.min.css", done));
gulp.task("clean", gulp.series(["clean:css"]));

gulp.task('min:styles', () => {
    return gulp.src(paths.styles + "BlazoredGitter.scss")
        .pipe(sass().on("error", sass.logError))
        .pipe(cssmin())
        .pipe(rename("blazored-gitter.min.css"))
        .pipe(gulp.dest(paths.concatCssDest))
});

gulp.task("min", gulp.series(["min:styles"]));

gulp.task("watch:styles", () => {
    gulp.watch("./Styles/*.scss", gulp.series("min:styles"));
});

gulp.task("default", gulp.series(["min"]));
