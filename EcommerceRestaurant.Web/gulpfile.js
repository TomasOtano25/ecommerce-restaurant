/// <binding AfterBuild='default' Clean='clean' />
"use strict";

var gulp = require('gulp');
var del = require('del');

var nodeRoot = './node_modules/';
var targetPath = './wwwroot/lib/';

gulp.task('clean:fontawesome-free', () => {
    return del([targetPath + '/fontawesome-free/**/*']);
});

gulp.task('clean:font-awesome', () => {
    return del([targetPath + '/font-awesome/**/*']);
});

gulp.task('clean', gulp.series(["clean:fontawesome-free", "clean:font-awesome"]));

gulp.task('default', async () => {
    gulp.src(nodeRoot + "font-awesome/**/*").pipe(gulp.dest(targetPath + "/font-awesome/"));
    gulp.src(nodeRoot + "@fortawesome/fontawesome-free/**/*").pipe(gulp.dest(targetPath + "/fontawesome-free/"));
});