#
# spec file for package python-atspi (Version 0.3.90)
#
# Copyright (c) 2010 SUSE LINUX Products GmbH, Nuernberg, Germany.
#
# All modifications and additions to the file contributed by third parties
# remain the property of their copyright owners, unless otherwise agreed
# upon. The license for this file, and modifications and additions to the
# file, is the same license as for the pristine package itself (unless the
# license for the pristine package is not an Open Source License, in which
# case the license is the MIT License). An "Open Source License" is a
# license that conforms to the Open Source Definition (Version 1.9)
# published by the Open Source Initiative.

# Please submit bugfixes or comments via http://bugs.opensuse.org/
#

%define IS_DEFAULT_ATSPI_STACK 0

Name:           python-atspi
%define _name   pyatspi
Version:        0.3.90
Release:        1
Summary:        Assistive Technology Service Provider Interface - Python bindings
License:        LGPLv2.0
Group:          Development/Libraries/Python
Url:            http://www.gnome.org/
Source0:        %{_name}-%{version}.tar.bz2
BuildRequires:  fdupes
BuildRequires:  python
Requires:       dbus-1-python
Requires:       python-gobject2
Requires:       python-gtk
# The bindings are really useful only if the at-spi registry is running. But
# it's not a strict runtime dependency.
Recommends:     at-spi2-core
# Old versions of at-spi 1.x provided the same files
Conflicts:      at-spi < 1.29.3
%if %IS_DEFAULT_ATSPI_STACK
# Virtual package, so that apps can depend on it, without having to know which
# at-spi stack is used. Only the default at-spi stack should define it.
Provides:       pyatspi
%endif
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
%if %suse_version > 1110
BuildArch:      noarch
%endif
%if %suse_version <= 1110
%define python_sitelib %{py_sitedir}
%endif
%py_requires

%description
AT-SPI is a general interface for applications to make use of the
accessibility toolkit. This version is based on dbus.

This package contains the python bindings for AT-SPI.

%prep
%setup -q -n %{_name}-%{version}

%build
# We pass --enable-relocate when we want another at-spi stack (like at-spi) by default.
%configure \
%if %IS_DEFAULT_ATSPI_STACK
        --disable-relocate
%else
        --enable-relocate
%endif
%__make %{?jobs:-j%jobs}

%install
%makeinstall
%fdupes %{buildroot}%{python_sitelib}

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc AUTHORS COPYING README
%if %IS_DEFAULT_ATSPI_STACK
%{python_sitelib}/pyatspi/
%else
%{python_sitelib}/pyatspi_dbus/
%endif

%changelog
