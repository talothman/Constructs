filter_angle = 75;
filter_radius = 150;
filter_depth = 90;
pi = 3.1415926543;

top_l = 2 * (2 * pi * filter_radius * filter_angle / 360 - 14);
top_d = top_l / pi;

lid_d = 100;

bottom_l = 55;
bottom_w = 10;

thickness = 4;

module holder_int() {
    sr = 1;
    sx1 = -bottom_l / 2 + sr;
    sy1 = -bottom_w / 2 + sr;
    sx2 =  bottom_l / 2 - sr;
    sy2 =  bottom_w / 2 - sr;

    nr = 24;
    rs = 360/nr;
    bs = bottom_l / nr * 2;

    hull() {
        translate([sx1, sy1, sr]) sphere(sr, $fn=32);
        translate([sx2, sy1, sr]) sphere(sr, $fn=32);
        translate([sx2, sy2, sr]) sphere(sr, $fn=32);
        translate([sx1, sy2, sr]) sphere(sr, $fn=32);

        translate([0, 0, filter_depth]) cylinder(d=top_d, h=1);
    }

    for(i=[1:nr]) {
        bx = i <= nr/2 ? bottom_l/2 - i * bs + bs/2: (i * bs - bottom_l/2 - bottom_l - bs/2);
        by = i <= nr/2 ? bottom_w/2: -bottom_w/2;

        hull() {
            translate([bx, by, -thickness]) sphere(3*sr, $fn=32);
            rotate([0, 0, i * rs - rs/2]) translate([top_d/2, 0, filter_depth]) sphere(sr*3, $fn=32);
        }
    }
}

module holder_ext() {
    sr = 1;
    sx1 = -bottom_l / 2 + sr - thickness;
    sy1 = -bottom_w / 2 + sr - thickness;
    sx2 =  bottom_l / 2 - sr + thickness;
    sy2 =  bottom_w / 2 - sr + thickness;

    hull() {

        translate([sx1, sy1, sr]) sphere(sr, $fn=32);
        translate([sx2, sy1, sr]) sphere(sr, $fn=32);
        translate([sx2, sy2, sr]) sphere(sr, $fn=32);
        translate([sx1, sy2, sr]) sphere(sr, $fn=32);

        translate([0, 0, filter_depth]) rotate_extrude() {
            union() {
                square([top_d/2 + thickness, sr * 2]);
                translate([top_d/2 + thickness, sr]) circle(sr, $fn=32);
            }
        }
    }
}

module lid() {
    sr = 2;
    lsr = 3;
    rotate_extrude() {
        union() {
            square([lid_d/2, sr * 2]);
            translate([lid_d/2, lsr]) circle(lsr, $fn=32);
        }
    }

}

$fn=120;

module holder1() {
    difference() {
        union() {
            holder_ext();
            lid();
        }
        translate([0, 0, thickness]) holder_int();
    }
}

module holder2_shape(d=50, nr=24, ofs=1) {
    rd1 = 2 * pi * d / nr / 2 + ofs;
    rd2 = 2 * pi * d / nr / 2 - ofs;
    rs = 360 / nr * 2;

    difference() {
        union() {
            circle(d=d);
            for(i=[1:nr]) {
                rotate([0, 0, rs * i]) translate([d/2, 0]) circle(d=rd2, $fn=32);
            }
        }

        for(i=[1:nr]) {
            rotate([0, 0, rs * i + rs/2]) translate([d/2, 0]) circle(d=rd1, $fn=32);
        }
    }
}

module holder2_holes(d=50, hd=1.5) {
    rotate_extrude() translate([d/2 + hd, 0]) circle(d=hd, $fn=32);
}

module holder2_drain(d=50, nr=24, ids=1, internal=false) {
    rd1 = 2 * pi * d / nr / 2 * ids;
    rd2 = 2 * pi * d / nr / 2 / ids;

    rs = 360 / nr * 2;
    $fn = 32;

    for(i=[1:nr]) {
        if(internal) {
            rotate([0, 0, rs * i]) translate([d/2, 0]) circle(d=rd2, $fn=32);
        } else {
            rotate([0, 0, rs * i + rs/2]) translate([d/2, 0]) circle(d=rd1, $fn=32);
        }
    }
}

module holder2() {
    eo = -1.5;
    io = 1.2;
    esc = (top_d + eo) / (bottom_l + eo);
    isc = (top_d + io) / (bottom_l + io);
    d1 = bottom_l;
    d2 = top_d;

    nr = 30;

    linear_extrude(height=filter_depth, scale=isc, twist=45) difference() {
        holder2_shape(d=bottom_l, nr=nr, ofs=eo);
        holder2_shape(d=bottom_l, nr=nr, ofs=io);
    }

    translate([0, 0, -1.9]) difference() {
        linear_extrude(height=2) holder2_shape(d=bottom_l, nr=nr, ofs=eo);
        translate([0, 0, -0.1]) linear_extrude(height=2.2) holder2_drain(d=d1, nr=nr, ids=1.3, internal=true);
        translate([0, 0, -0.1]) linear_extrude(height=2.2) holder2_drain(d=d1 * 2/3, nr=nr * 2/3, ids=1.3, internal=true);
        translate([0, 0, -0.1]) linear_extrude(height=2.2) holder2_drain(d=d1 * 1/3, nr=nr * 1/3, ids=1.3, internal=true);
    }
}


module holder3() {
    eo = -1;
    io = 1;
    isc = (top_d - io) / (bottom_l - io);
    esc = (top_d + eo) / (bottom_l + eo);
    db = bottom_l;
    dt = top_d;

    nr = 30;

    difference() {
        linear_extrude(height=filter_depth, scale=esc, twist=45) holder2_shape(d=bottom_l, nr=nr, ofs=eo);
        intersection() {
            translate([-top_d/2 - 8, -top_d/2 - 8, 2]) cube(top_d + 16);
            linear_extrude(height=filter_depth+0.1, scale=isc, twist=45) holder2_shape(d=bottom_l, nr=nr, ofs=io);
        }

        translate([0, 0, -0.1]) linear_extrude(height=filter_depth, scale=isc, twist=45) holder2_drain(d=db, nr=nr, ids=1.3, internal=true);
        translate([0, 0, -0.1]) linear_extrude(height=4.2) holder2_drain(d=db * 2/3, nr=nr * 2/3, ids=1.3, internal=true);
        translate([0, 0, -0.1]) linear_extrude(height=4.2) holder2_drain(d=db * 1/3, nr=nr * 1/3, ids=1.3, internal=true);
    }
}

holder3();
